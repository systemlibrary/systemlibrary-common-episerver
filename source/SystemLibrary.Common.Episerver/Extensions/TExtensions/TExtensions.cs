using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

using React;

using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

public static partial class TExtensions
{
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

    public static StringBuilder ReactServerSideRender<T>(this T propsModel, Type propsModelType, object additionalProps = null, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null, bool renderClientOnly = false, bool renderServerOnly = false) where T : class
    {
        if (tagName?.StartsWith("<") == true)
            throw new Exception("'tagName' should not include < > characters");

        if (tagName.IsNot() && renderClientOnly)
            throw new Exception("'tagName' must be included when using 'renderClientOnly', as it is used to keep track of the initial properties. Please set it to for instance 'div'");

        if (tagName != null && tagName != "div" && tagName != "article" && tagName != "section" && tagName != "header" && tagName != "footer")
            throw new Exception("'tagName' must be either null, div, section, header, article or footer");

        if (additionalProps != null && !additionalProps.GetOriginalType().IsClass)
            throw new Exception("'additionalProps' passed must be a class with C# properties, where they will be passed as props into your react component. If an additional property matches a property in the model, then additional property is overwriting it");

        if (renderServerOnly && renderClientOnly)
            throw new Exception("Choose either client or server side rendering, or render on both sides, by setting both to false: " + componentFullName);

        var content = new StringBuilder();

        var renderServerSide = !renderClientOnly || renderServerOnly;
        var renderClientSide = renderClientOnly || !renderServerOnly;

        if (tagName.Is())
            content.Append("<" + tagName);
        else
        {
            //TODO: if tag name is not, and server side?
            //Then use the ID and CSSCLASS directly on the element AFTER server side rendered ?
            //If render ClientOnly, we render tag, id and css class of course ...
            //or I cant? As the react hydration then, does???
        }

        try
        {
            if (tagName.Is())
            {
                if (id.Is())
                    content.Append(" id=\"" + id + "\"");

                if (cssClass.Is())
                    content.Append($" class=\"{cssClass}\"");
            }
      
            if (renderClientSide)
            {
                if (!HttpContextInstance.Current.Items.ContainsKey(SysLibComponentArgs))
                {
                    HttpContextInstance.Current.Items.Add(SysLibComponentArgs, new StringBuilder());
                }
                if (!HttpContextInstance.Current.Items.ContainsKey(SysLibComponentKeys))
                {
                    HttpContextInstance.Current.Items.Add(SysLibComponentKeys, new List<string>());
                }

                if (HttpContextInstance.Current.Items.ContainsKey(SysLibComponentLevel))
                {
                    HttpContextInstance.Current.Items[SysLibComponentLevel] = (int)HttpContextInstance.Current.Items[SysLibComponentLevel] + 1;
                }
                else
                {
                    HttpContextInstance.Current.Items.Add(SysLibComponentLevel, 0);
                }
            }

            (var props, var key) = ToReactComponentProps(propsModel, additionalProps, camelCaseProps, id);

            var level = 0;

            if (renderClientSide)
                level = (int)HttpContextInstance.Current.Items[SysLibComponentLevel];

            IReactComponent component = null;

            var options = new JsonSerializerOptions()
            {
                IncludeFields = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                MaxDepth = 16,
                AllowTrailingCommas = true,
                WriteIndented = false,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = camelCaseProps ? JsonNamingPolicy.CamelCase : null,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonFormattedProps = new StringBuilder(props.Json(options));

            if (renderClientSide)
            {
                var keys = HttpContextInstance.Current.Items[SysLibComponentKeys] as List<string>;

                // NOTE: keys are a list of already printed <input type=hidden props...>
                if (!keys.Contains(key))
                {
                    keys.Add(key);

                    var componentProps = HttpUtility.HtmlAttributeEncode(jsonFormattedProps.ToString());

                    var componentPropsList = HttpContextInstance.Current.Items[SysLibComponentArgs] as StringBuilder;

                    componentPropsList.Append($"<input type='hidden' id=\"" + key + "-" + level + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{componentProps}\" />" + Environment.NewLine);
                }

                if (tagName.Is())
                    content.Append(" data-rcssr-id=\"" + key + "-" + level + "\"");
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
                    var space = content.IndexOf(" ");
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
            HttpContextInstance.Current.Items[SysLibComponentLevel] = (int)HttpContextInstance.Current.Items[SysLibComponentLevel] - 1;
        }

        return content;
    }

    // TODO: Skip adding props to dictionaries, directly write to a 'JsonWriter' which contains a "stringbuilder" behind the scenes
    // would "double" the performance in "one optimization"
    // Basically: ReactComponentResult should create a StringBuilder/JsonWriter, and reuse it throughout all components and properties rendered
    // By passing the ref "downwards" and around...
    static (ExpandoObject, string) ToReactComponentProps(object model, object additionalProps, bool forceCamelCase = false, string id = null)
    {
        IDictionary<string, object> props = model.ToExpandoObject(forceCamelCase);

        var propCount = props.Count();

        var key = new StringBuilder();

        var hasAppendedKey = false;

        if (model is IContent icontent)
        {
            hasAppendedKey = true;
            key.Append("i" + icontent?.ContentLink?.ID + "_" + icontent?.ContentLink?.WorkID);
        }
        else if (id.Is())
        {
            hasAppendedKey = true;
            key.Append(propCount + "_" + id);
        }
        else
        {
            var typeName = model?.GetType()?.Name;
            if (typeName.EndsWith("_DynamicProxy"))
                key.Append(propCount + "_Dyn");
            else
                key.Append(propCount + "_" + typeName);

            if (HttpContextInstance.Current.Items.ContainsKey(SysLibComponentLevel))
                key.Append("_" + HttpContextInstance.Current.Items[SysLibComponentLevel]);
        }

        for (int i = 0; i < propCount; i++)
        {
            var property = props.ElementAt(i);

            if (property.Value == null) continue;

            if (!hasAppendedKey)
            {
                if (property.Value is int number)
                {
                    key.Append("_" + number);
                }
                else if (property.Value is bool b)
                {
                    key.Append("_" + (b ? "1" : "0"));
                }
                else if (property.Value is Enum e)
                {
                    key.Append("_" + e.ToValue());
                }
                else if (property.Value is string text)
                {
                    if (text?.Length > 5 && text.Length <= 255)
                    {
                        hasAppendedKey = true;
                        if (key.Length > 28)
                        {
                            key.Append("_" + text.Substring(0, 28).ToBase64().Replace("+", "").Replace("=", ""));
                        }
                        else
                        {
                            key.Append("_" + text.ToBase64().Replace("+", "").Replace("=", ""));
                        }
                    }
                }
                else if (property.Value is StringBuilder sb)
                {
                    if (sb?.Length > 5 && sb?.Length <= 255)
                    {
                        hasAppendedKey = true;
                        if (sb.Length > 28)
                        {
                            key.Append("_" + sb.ToString().Substring(0, 28).ToBase64().Replace("+", "").Replace("=", ""));
                        }
                        else
                        {
                            key.Append("_" + sb.ToString().ToBase64().Replace("+", "").Replace("=", ""));
                        }
                    }

                    props[property.Key] = sb?.ToString();
                }
            }
            else
            {
                if (property.Value is StringBuilder sb)
                {
                    props[property.Key] = sb?.ToString();
                }
            }
        }

        IDictionary<string, object> additional = additionalProps.ToExpandoObject(forceCamelCase);

        if (additional != null)
        {
            key.Append("_p");

            foreach (var kv in additional)
            {
                if (props.ContainsKey(kv.Key))
                    props.Remove(kv.Key);

                props.Add(kv.Key, kv.Value);
            }
        }

        return ((ExpandoObject)props, key.ToString());
    }

    public class ComponentArgsList
    {
        public static Dictionary<int, List<string>> Args = new Dictionary<int, List<string>>();

        public static void Clear()
        {
            if (Args.Count == 0) return;

            foreach (var list in Args)
            {
                list.Value.Clear();
            }
            Args.Clear();
            Args = new Dictionary<int, List<string>>();
        }

        public static void Add(string value)
        {
            var depth = (int)HttpContextInstance.Current.Items["__sysLevel"];

            if (depth <= 1) return;

            if (!Args.ContainsKey(depth))
            {
                Args.Add(depth, new List<string>());
            }

            Args[depth].Add(value);
        }

        public static IEnumerable<string> GetComponentArgsToRemove(int currentDepth)
        {
            if (!Args.ContainsKey(currentDepth)) yield break;

            foreach (var componentArgs in Args[currentDepth])
            {
                yield return componentArgs;
            }
        }
    }
}
