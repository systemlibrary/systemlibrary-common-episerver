using System;
using System.Text;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.Web.Mvc;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary;
using SystemLibrary.Common;
using SystemLibrary.Common.Episerver;
using SystemLibrary.Common.Episerver.Cms;
using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Components;

public abstract class AsyncComponent<T> : AsyncBlockComponent<T> where T : BlockData
{
    const string WindowReactComponentsPath = "reactComponents";

    protected async Task<IViewComponentResult> ResultAsync(object component, string viewName = "Index.cshtml")
    {
        if (!viewName.StartsWith("~"))
        {
            var componentName = component.GetType().Name;

            var componentType = componentName
                .Replace("Proxy", "")
                .Replace("ViewModel", "")
                .Replace("Model", "");

            if(componentName.Contains("Block"))
                viewName = "~/Content/Blocks/" + componentType + "/" + viewName;
            else
                viewName = "~/Content/Components/" + componentType + "/" + viewName;
        }

        if (!viewName.EndsWith(".cshtml"))
        {
            if (viewName.EndsWith("/"))
                return await Task.FromResult(View(viewName + "Index.cshtml", component));
            else
                return await Task.FromResult(View(viewName + ".cshtml", component));
        }

        return await Task.FromResult(View(viewName, component));
    }

    protected IViewComponentResult Result(object component, string viewName = "Index.cshtml")
    {
        if (!viewName.StartsWith("~"))
        {
            var componentType = component.GetType().Name.Replace("ViewModel", "").Replace("Model", "");

            viewName = "~/Content/Blocks/" + componentType + "/" + viewName;
        }

        if (!viewName.EndsWith(".cshtml"))
        {
            if (viewName.EndsWith("/"))
                return View(viewName + "Index.cshtml", component);
            else
                return View(viewName + ".cshtml", component);
        }

        return View(viewName, component);
    }

    protected async Task<IViewComponentResult> ReactServerSideResultAsync(object model, object additionalProps = null, bool camelCaseProps = false, bool renderClientOnly = false, bool renderServerOnly = false, string tagName = "div", string cssClass = null, string id = null, string componentFullName = null)
    {
        return await Task.FromResult(ReactServerSideResult(model, additionalProps, camelCaseProps, renderClientOnly, renderServerOnly, tagName, cssClass, id, componentFullName));
    }

    protected IViewComponentResult ReactServerSideResult(object model, object additionalProps = null, bool camelCaseProps = false, bool renderClientOnly = false, bool renderServerOnly = false, string tagName = "div", string cssClass = null, string id = null, string componentFullName = null)
    {
        try
        {
            var type = GetType(model);

            componentFullName = GetReactComponentFullName(type, componentFullName);

            var data = model.ReactServerSideRender(type, additionalProps, tagName, camelCaseProps, cssClass, id, componentFullName, renderClientOnly, renderServerOnly);

            if (!renderServerOnly)
                AppendClientProperties(data);

            return new HtmlContentViewComponentResult(new HtmlString(data.ToString()));
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return null;
        }
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
