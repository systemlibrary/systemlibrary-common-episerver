﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Html Helper factory, to create a HtmlHelper instance
/// </summary>
public class HtmlHelperFactory
{
    //Creds to: https://stackoverflow.com/questions/42039269/create-custom-html-helper-in-asp-net-core/51466436#51466436
    static ModelStateDictionary ModelStateDictionary = new ModelStateDictionary();
    static HtmlHelperOptions HtmlHelperOptions = new HtmlHelperOptions();
    static ControllerActionDescriptor ControllerActionDescriptor = new ControllerActionDescriptor();
    static DummyIndexView DummyIndex = new DummyIndexView();
    static ITempDataProvider TempDataProvider;

    static IHttpContextAccessor _HttpContextAccessor;

    static IHttpContextAccessor HttpContextAccessor
    {
        get
        {
            if (_HttpContextAccessor == null)
            {
                _HttpContextAccessor = Services.Get<IHttpContextAccessor>();
            }
            return _HttpContextAccessor;
        }
    }

    /// <summary>
    /// Create a generic Html Helper instance
    /// </summary>
    public static IHtmlHelper<T> Build<T>() where T : class
    {
        var viewContext = GetViewContext<T>(HttpContextAccessor);

        var htmlHelper = Services.Get<IHtmlHelper<T>>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);

        return htmlHelper;
    }

    /// <summary>
    /// Create a non-generic Html Helper instance
    /// </summary>
    public static IHtmlHelper Build()
    {
        var viewContext = GetViewContext(HttpContextAccessor);

        var htmlHelper = Services.Get<IHtmlHelper>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);

        return htmlHelper;
    }

    static ViewContext GetViewContext<T>(IHttpContextAccessor contextAccessor)
    {
        var modelMetadataProvider = contextAccessor.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var viewData = new ViewDataDictionary<T>(modelMetadataProvider, ModelStateDictionary);

        return GetViewContext(contextAccessor, viewData);
    }

    static ViewContext GetViewContext(IHttpContextAccessor contextAccessor)
    {
        var modelMetadataProvider = contextAccessor.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var viewData = new ViewDataDictionary(modelMetadataProvider, ModelStateDictionary);

        return GetViewContext(contextAccessor, viewData);
    }

    static ViewContext GetViewContext(IHttpContextAccessor contextAccessor, ViewDataDictionary viewData)
    {
        if (TempDataProvider == null)
            TempDataProvider = contextAccessor.HttpContext.RequestServices.GetRequiredService<ITempDataProvider>();

        var tempData = new TempDataDictionary(contextAccessor.HttpContext, TempDataProvider);

        return new ViewContext(
            new ActionContext(contextAccessor.HttpContext, contextAccessor.HttpContext.GetRouteData(), ControllerActionDescriptor),
            DummyIndex,
            viewData,
            tempData,
            TextWriter.Null,
            HtmlHelperOptions
        );
    }

    internal class DummyIndexView : IView
    {
        public Task RenderAsync(ViewContext context)
        {
            return Task.CompletedTask;  // NOTE: This is more performant than Task.FromResult(0)
        }

        public string Path => "Index";
    }
}
