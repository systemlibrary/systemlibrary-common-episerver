using System.Text;

using EPiServer;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver;

public class ReactComponentResult : ContentResult
{
    const string WindowReactComponentsPath = "reactComponents";

    public ReactComponentResult(object model, object additionalProps = null, bool camelCaseProps = false, bool renderClientOnly = false, bool renderServerOnly = false, string tagName = "div", string cssClass = null, string id = null, string componentFullName = null)
    {
        var type = GetType(model);

        componentFullName = GetReactComponentFullName(type, componentFullName);

        ContentType = "text/html";

        var data = model.ReactServerSideRender(type, additionalProps, tagName, camelCaseProps, cssClass, id, componentFullName, renderClientOnly, renderServerOnly);

        if (!renderServerOnly)
            AppendClientProperties(data);

        Content = data.ToString();
    }

    static string GetReactComponentFullName(Type modelType, string componentFullName)
    {
        if (componentFullName.Is()) return componentFullName;

        return WindowReactComponentsPath + "." + GetReactComponentName(modelType);
    }

    static Type GetType(object model)
    {
        var type = model.GetOriginalType();

        if (!type.IsClass)
            throw new Exception("'viewModel/model' passed must be a class with C# properties, where they will be passed as props into your react component");

        return type;
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

    static void AppendClientProperties(StringBuilder data)
    {
        if (HttpContextInstance.Current?.Items?.ContainsKey(TExtensions.SysLibComponentLevel) != true)
            return;

        var level = (int)HttpContextInstance.Current.Items[TExtensions.SysLibComponentLevel];
        if (level != -1)
            return;

        if (HttpContextInstance.Current?.Items?.ContainsKey(TExtensions.SysLibComponentArgs) != true)
            return;

        var reactComponentProps = HttpContextInstance.Current.Items[TExtensions.SysLibComponentArgs] as StringBuilder;
        if (reactComponentProps?.Length > 0)
        {
            data.Append(reactComponentProps);
            reactComponentProps.Clear();
        }
    }
}