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
        var data = model.ReactServerSideRender(additionalProps, tagName, camelCaseProps, cssClass, id, componentFullName, renderClientOnly, renderServerOnly);

        return new HtmlContentViewComponentResult(new HtmlString(data.ToString()));
    }
}
