using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

using React;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class TExtensions
{
    /// <summary>
    /// Tries to render the react component server side, where 'viewModel.GetType().Name' is the name of the component
    /// - if GetType().Name contains "ViewModel" or "Model" those are removed
    /// - the remaining Name then must match a property of the GlobalThisServerSideObject
    /// - GlobalThisServerSideObject default value is 'reactComponents'
    /// 
    /// Returns always a string, minimum empty string
    /// </summary>
    internal static string ServerSideRenderReactComponent<T>(this T viewModel, object additionalProps, string tag = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null)
    {
        if (viewModel == null && additionalProps == null)
        {
            return "";
        }

        var type = typeof(T);
        if (!type.IsClass)
            throw new Exception("'viewModel/model' passed must be a class with C# properties, where they will be passed as props into your react component");

        if(additionalProps != null && !additionalProps.GetType().IsClass)
            throw new Exception("'additionalProps' passed must be a class with C# properties, where they will be passed as props into your react component");

        var componentName = GetReactComponentName(type);
        if (componentFullName.IsNot())
        {
            var componentFolder = GetGlobalThisVariablePath();
            componentFullName = componentFolder + "." + componentName;
        }
        try
        {
            componentFullName = "modules.breadcrumbs.BreadCrumbs";
            var props = ToReactComponentProps(viewModel, additionalProps, camelCaseProps);

            var data = new StringBuilder();
            if (id.Is())
                tag += " id=\"{id}\"";

            var options = new JsonSerializerOptions()
            {
                MaxDepth = 16,
                AllowTrailingCommas = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = false,
            };

            if (cssClass.Is())
            {
                data.Append($"<{tag} class=\"{cssClass}\" data-react-hydrate=\"{componentFullName}\" data-react-hydrate-props=\"{HttpUtility.HtmlAttributeEncode(JsonSerializer.Serialize(props, options))}\">");
            }
            else
            {
                string componentContainerClassName = GetComponentCssClass(componentName);

                data.Append($"<{tag} class=\"{componentContainerClassName}\" data-react-hydrate=\"{componentFullName}\" data-react-hydrate-props=\"{HttpUtility.HtmlAttributeEncode(JsonSerializer.Serialize(props, options))}\">");
            }

            var component = ReactEnvironment.GetCurrentOrThrow.CreateComponent(componentFullName, props, null, false, true);
            component.Props = props;
            component.ContainerClass = cssClass;
            component.ContainerTag = tag;

            data.Append(component.RenderHtml(renderContainerOnly: false, true));

            data.Append($"</{tag}>");

            return data.ToString();
        }
        catch (Exception ex)
        {
            Log.Write(ex);
            if (ex?.InnerException != null)
                Log.Write(ex?.InnerException?.ToString());

            if (ex?.InnerException?.InnerException != null)
                Log.Write(ex?.InnerException?.InnerException?.ToString());

            return "<" + tag + ">" + ex.ToString() + "</" + tag + ">";
        }
        finally
        {
            ReactEnvironment.GetCurrentOrThrow.ReturnEngineToPool();
        }
    }

    static string GetComponentCssClass(string componentName)
    {
        var temp = Regex.Replace(componentName, "([A-Z])", "-$1");

        if (temp[0] == '-')
            return temp.Substring(1).ToLower() + "--component";

        return temp.ToLower() + "--component";
    }

    static ExpandoObject ToReactComponentProps(object model, object additionalProps, bool forceCamelCase = false)
    {
        IDictionary<string, object> props = model.ToExpandoObject(forceCamelCase);
        IDictionary<string, object> additional = additionalProps.ToExpandoObject(forceCamelCase);

        if (additional != null)
        {
            foreach (var kv in additional)
            {
                if (props.ContainsKey(kv.Key))
                    props.Remove(kv.Key);

                props.Add(kv.Key, kv.Value);
            }
        }

        return (ExpandoObject)props;
    }

    static string GetGlobalThisVariablePath()
    {
        return "reactComponents";
    }

    static string GetReactComponentName(Type type)
    {
        var name = type.Name;

        if (name.EndsWith("ViewModel"))
            return name.Substring(0, name.Length - "ViewModel".Length);

        if (name.EndsWith("Model"))
            return name.Substring(0, name.Length - "Model".Length);

        return name;
    }
}
