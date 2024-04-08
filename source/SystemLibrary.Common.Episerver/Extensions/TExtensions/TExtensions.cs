using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client.Extensions.Msal;

using React;

using SystemLibrary.Common.Episerver.Cms.Properties;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystemLibrary.Common.Episerver.Extensions;

public static partial class TExtensions
{
    const string WindowComponentsPath = "reactComponents";

    static FieldInfo __props;
    static FieldInfo _props
    {
        get
        {
            if (__props == null)
                __props = typeof(ReactComponent).GetField("_props", BindingFlags.Instance | BindingFlags.NonPublic);

            return __props;
        }
    }
    static FieldInfo __serializedProps;
    static FieldInfo _serializedProps
    {
        get
        {
            if (__serializedProps == null)
                __serializedProps = typeof(ReactComponent).GetField("_serializedProps", BindingFlags.Instance | BindingFlags.NonPublic);

            return __serializedProps;
        }
    }

    internal const string SysLibComponentLevel = "___" + nameof(SysLibComponentLevel);
    internal const string SysLibComponentArgs = "___" + nameof(SysLibComponentArgs);
    internal const string SysLibComponentKeys = "___" + nameof(SysLibComponentKeys);

    /// <summary>
    /// Render a model server side and return the HTML rendered, in a StringBuilder.
    /// 
    /// Throws exception if you pass invalid params.
    /// </summary>
    /// <param name="propsModel">Current component/block in the CMS</param>
    /// <param name="additionalProps">Override or append properties to the model</param>
    /// <param name="tagName">Usually div, can be section, article, header, footer or null</param>
    /// <param name="camelCaseProps">Set to 'true' if you want to always send camel cased props to your components, else it uses the C# property names as is</param>
    /// <param name="cssClass">null to not print it, leave it blank to auto-add '-container', or pass in your own custom css class, attached to the 'outer element' of your component</param>
    /// <param name="id"></param>
    /// <param name="componentFullName">If not set, uses 'reactComponents.' as the global path where your components should live, else add its full name</param>
    /// <param name="renderClientOnly">Skip rendering server side, only printing the minimal DOM to let your javascript do the rendering</param>
    /// <param name="renderServerOnly">Skip printing client side props, only prints the components at serverside. For components that just prints text or 'a href', no javascript events...</param>
    public static StringBuilder ReactServerSideRender<T>(this T propsModel, object additionalProps = null, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null, bool renderClientOnly = false, bool renderServerOnly = false, bool debug = false) where T : class
    {
        var content = new StringBuilder();

        if (propsModel == null) return content;

        var type = propsModel.GetOriginalType();

        if (!type.IsClass)
            throw new Exception("'viewModel/model' passed must be a class with C# properties, where they will be passed as props into your react component");

        if (componentFullName.IsNot())
            componentFullName = WindowComponentsPath + "." + GetComponentName(type);

        if (tagName?.StartsWith("<") == true)
            throw new Exception("'tagName' should not include < > characters");

        if (tagName.IsNot() && renderClientOnly)
            throw new Exception("'tagName' must be included when using 'renderClientOnly', as it is used to keep track of the initial properties. Please set it, for instance: 'div'");

        if (tagName != null && tagName != "div" && tagName != "article" && tagName != "section" && tagName != "header" && tagName != "footer")
            throw new Exception("'tagName' must be either null, div, section, header, article or footer");

        if (additionalProps != null && !additionalProps.GetOriginalType().IsClass)
            throw new Exception("'additionalProps' passed must be a class with C# properties, where they will be passed as props into your react component. If an 'additional property' matches a property in the model, then the 'additional property' is overwriting it");

        if (renderServerOnly && renderClientOnly)
            throw new Exception("You cannot render 'client only' and 'server side only', it doesnt make any sense. Choose either client or server side or let both be false, to render on both sides: " + componentFullName);

        var renderServerSide = !renderClientOnly || renderServerOnly;
        var renderClientSide = renderClientOnly || !renderServerOnly;

        if (tagName.Is())
            content.Append("<" + tagName);

        var storage = HttpContextInstance.Current.Items;

        try
        {
            if (tagName.Is())
            {
                if (id.Is())
                    content.Append(" id=\"" + id + "\"");

                if (cssClass == "")
                    content.Append(" class=\"" + GetComponentContainerClassName(componentFullName) + "\"");
                else if (cssClass != null)
                    content.Append($" class=\"{cssClass}\"");
                // cssClass is null, default to leave it out
            }

            if (renderClientSide)
            {
                if (!storage.ContainsKey(SysLibComponentArgs))
                {
                    storage.Add(SysLibComponentArgs, new StringBuilder());
                }
                if (!storage.ContainsKey(SysLibComponentKeys))
                {
                    storage.Add(SysLibComponentKeys, new List<string>());
                }

                if (storage.ContainsKey(SysLibComponentLevel))
                {
                    var nextLevel = (int)storage[SysLibComponentLevel] + 1;
                    if (nextLevel > 256)
                    {
                        return new StringBuilder("<p color='red'>Components nested too deply: max depth 256</p>");
                    }

                    storage[SysLibComponentLevel] = nextLevel;
                }
                else
                {
                    storage.Add(SysLibComponentLevel, 0);
                }
            }

            (var props, var key) = ToReactComponentProps(storage, propsModel, additionalProps, camelCaseProps, id);

            var level = 0;

            if (renderClientSide)
                level = (int)storage[SysLibComponentLevel];

            IReactComponent component = null;

            var options = new JsonSerializerOptions()
            {
                IncludeFields = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                MaxDepth = 32,
                AllowTrailingCommas = true,
                WriteIndented = false,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = camelCaseProps ? JsonNamingPolicy.CamelCase : null,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonProps = props.Json(options);
            var jsonFormattedProps = new StringBuilder(jsonProps);

            if (renderClientSide)
            {
                key = key + jsonProps.Length;
                var keys = storage[SysLibComponentKeys] as List<string>;

                // NOTE: keys are a list of already printed <input type=hidden props...>
                if (!keys.Contains(key))
                {
                    keys.Add(key);

                    var componentProps = HttpUtility.HtmlAttributeEncode(jsonFormattedProps.ToString());

                    var componentPropsList = storage[SysLibComponentArgs] as StringBuilder;

                    componentPropsList.Append($"<input type='hidden' id=\"" + key + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{componentProps}\" />" + Environment.NewLine);

                    if (debug)
                        Log.Debug(obj: "Appending: input id="  + key);
                }
                else
                {
                    if (debug)
                        Log.Debug(obj: "Skipping: input id=" + key + " as it already was in the list");
                }

                if (tagName.Is())
                    content.Append(" data-rcssr-id=\"" + key + "\"");
            }

            if (tagName.Is())
                content.Append(">");

            if (renderServerSide)
            {
                // NOTE: Sending dummy data as CreateComponent immediately serializes the props using Newtonsoft
                // NOTE: System.Text.Json uses less memory, and is faster
                // NOTE: For client side, we also need json serialized, adding it to the attribute 'data-rcssr-props'
                // NOTE: Newtonsoft's default escaping is not sufficient, and EscapeHtml generates 10% more text than System.Text.Json

                var dummy = new Dictionary<string, object>();
                component = ReactEnvironment.GetCurrentOrThrow.CreateComponent(componentFullName, dummy, null, false, true, true);
                component.ContainerClass = null;
                component.ContainerTag = tagName.Is() ? null : "div";

                _props.SetValue(component, props);
                _serializedProps.SetValue(component, jsonFormattedProps.ToString());

                content.Append(component.RenderHtml(false, true));

                if (tagName.IsNot() && renderClientSide)
                {
                    var space = content?.IndexOf(" ") ?? 0;
                    if (space > 1 && space < 10)
                    {
                        content.Insert(space, " data-rcssr-id=\"" + key + "-" + level + "\"");
                    }
                }
            }

            if (tagName.Is())
                content.Append($"</{tagName}>");
        }
        catch (Exception ex)
        {
            var message = ex.ToString();
            if (ex.InnerException != null)
                message = ex.Message + " " + ex.InnerException;

            if (ex?.InnerException?.InnerException != null)
                message = ex.Message + " " + ex.InnerException.Message + " " + ex.InnerException.InnerException;

            Log.Error(message);

            content.Clear();
            content.Append("<" + tagName + " data-rcssr-error='true'>" + message + "</" + tagName + ">");
        }
        finally
        {
            try
            {
                ReactEnvironment.GetCurrentOrThrow.ReturnEngineToPool();
            }
            catch (Exception ex)
            {
                Log.Error("React returning engine too pool failed, continue silently..." + ex.Message);
            }
        }

        if (renderClientSide)
        {
            storage[SysLibComponentLevel] = (int)storage[SysLibComponentLevel] - 1;
        }

        if (!renderServerOnly)
            AppendClientProperties(storage, content);

        return content;
    }

    static string GetComponentName(Type type)
    {
        var name = type.Name;

        if (name.EndsWith("ViewModel"))
            return name.Substring(0, name.Length - "ViewModel".Length);

        if (name.EndsWith("Model"))
            return name.Substring(0, name.Length - "Model".Length);

        return name;
    }

    static void AppendClientProperties(IDictionary<object, object> storage, StringBuilder data)
    {
        if (storage?.ContainsKey(TExtensions.SysLibComponentLevel) != true)
            return;

        var reactComponentProps = storage[TExtensions.SysLibComponentArgs] as StringBuilder;
        if (reactComponentProps?.Length > 0)
        {
            data.Append(reactComponentProps);
            reactComponentProps.Clear();
        }

        var level = (int)storage[TExtensions.SysLibComponentLevel];
        if (level <= -1)
        {
            //var keys = storage[SysLibComponentKeys] as List<string>;
            //i//(keys?.Count > 0)
            //    keys.Clear();

            //Lo//Debug(obj: "Component level: < -1,//learing keys so inputs might dupl//ate");
        }
    }

    // TODO: Skip adding props to a dictionary, simply write it to a StringBuilder directly
    // This little "trick" would half the memory usage "right there"
    static (ExpandoObject, string) ToReactComponentProps(IDictionary<object, object> storage, object model, object additionalProps, bool forceCamelCase = false, string id = null)
    {
        IDictionary<string, object> props = model.ToExpandoObject(forceCamelCase);

        var propCount = props.Count();

        var key = new StringBuilder();

        var keyIsUnique = false;

        if (model is IContent icontent)
        {
            keyIsUnique = true;
            key.Append("k-" + propCount + "-" + icontent?.ContentLink?.ID + "-" + icontent?.ContentLink?.WorkID);
        }
        else if (id.Is())
        {
            keyIsUnique = true;
            key.Append("k-" + propCount + "-" + id);
        }
        else
        {
            // TODO: If the model is a "ViewModel" which contains "CurrentBlock"
            // then we can fetch the content Id from property "CurrentBlock" on the 'model'
            var contentData = model as ContentData;
            if (contentData != null)
            {
                key.Append("k-" + contentData.ContentTypeID);
            }
            else
            {
                key.Append("k-" + model.GetType()?.Name
                    .Replace("DynamicProxy", "D")
                    .Replace("AnonymousType", "AT")
                    .Replace("<>", "")
                    .Replace("`", ""));
            }
        }

        for (int i = 0; i < propCount; i++)
        {
            var property = props.ElementAt(i);

            if (property.Value == null)
            {
                if (!keyIsUnique)
                    key.Append("o");

                continue;
            }

            if (!keyIsUnique)
            {
                if (property.Value is StringBuilder sb)
                {
                    props[property.Key] = sb?.ToString();

                    if (sb?.Length > 0)
                    {
                        if (sb?.Length > 5)
                        {
                            key.Append("s" + sb.Length + "" + sb[3] + sb[4] + sb[sb.Length - 5]);
                        }
                        else if (sb.Length > 1)
                        {
                            key.Append("s" + sb.Length + "" + sb[0] + sb[1]);
                        }
                        else
                            key.Append("s");
                    }
                }

                else if (property.Value is string txt)
                {
                    if (txt?.Length > 5)
                    {
                        key.Append("t" + txt.Length + "" + txt[3] + "" + txt[4] + "" + txt[txt.Length - 5]);
                    }
                    else if (txt.Length > 1)
                    {
                        key.Append("t" + txt?.Length + "" + txt[0] + "" + txt[1]);
                    }
                    else
                        key.Append("t");
                }

                else if (property.Value is int number)
                    key.Append("i" + number);

                else if (property.Value is bool b)
                    key.Append("b" + (b ? "A" : "B"));

                else if (property.Value is ContentReference cr)
                    key.Append("cr" + cr?.ID + +cr?.WorkID);

                else if (property.Value is Url u)
                    key.Append("u" + u?.ToString()?.Length);

                else if (property.Value is Enum e)
                    key.Append("E" + e.ToString());

                else if (property.Value is ContentArea ca)
                    key.Append("ca" + ca?.Count);

                else if (property.Value is IEnumerable en)
                    key.Append(property.Key[0] + "en");

                else if (property.Value is DateTime dt)
                    key.Append("dt" + dt.Day);

                else
                {
                    key.Append(property.Key[0]);
                }

                if (key.Length > 64)
                {
                    keyIsUnique = true;
                    key = key.Replace("<", "_")
                      .Replace("&", "WW")
                      .Replace(">", "_")
                      .Replace("`", "")
                      .Replace("'", "")
                      .Replace("\n", "")
                      .Replace(" ", "z")
                      .Replace("/", "--")
                      .Replace("\\", "__")
                      .Replace(":", "QQ")
                      .Replace(";", "ZZ")
                      .Replace(";", "_")
                      .Replace(Environment.NewLine, "")
                      .Replace("\"", ".");
                }
            }
            else if (property.Value is StringBuilder sb)
                props[property.Key] = sb?.ToString();
        }

        IDictionary<string, object> additional = additionalProps.ToExpandoObject(forceCamelCase);

        if (additional != null)
        {
            foreach (var kv in additional)
            {
                if (props.ContainsKey(kv.Key))
                    props.Remove(kv.Key);

                props.Add(kv.Key, kv.Value);
            }

            key.Append("-" + additional.Count);
        }

        if (!keyIsUnique)
        {
            key = key.Replace("<", "_")
                      .Replace("&", "WW")
                      .Replace(">", "_")
                      .Replace("`", "")
                      .Replace("'", "")
                      .Replace("\n", "")
                      .Replace(" ", "z")
                      .Replace("/", "--")
                      .Replace("\\", "__")
                      .Replace(":", "QQ")
                      .Replace(";", "ZZ")
                      .Replace(";", "_")
                      .Replace(Environment.NewLine, "")
                      .Replace("\"", ".");
        }


        return ((ExpandoObject)props, key.ToString());
    }


    static string GetComponentContainerClassName(string componentFullName)
    {
        var index = componentFullName.LastIndexOf('.');

        var componentName = componentFullName.Substring(index + 1);

        var temp = Regex.Replace(componentName, "([A-Z])", "-$1");

        if (temp[0] == '-')
            return temp.Substring(1).ToLower() + "-container";

        return temp.ToLower() + "-container";
    }
}
