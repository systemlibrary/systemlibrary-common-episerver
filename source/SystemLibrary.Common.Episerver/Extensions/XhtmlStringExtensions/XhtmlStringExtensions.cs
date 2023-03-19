using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Web.Mvc.Html;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Common xhtmlstring extensions
/// </summary>
public static class XhtmlStringExtensions
{
    /// <summary>
    /// Returns true if the XhtmlString contains some value, else false
    /// 
    /// Note: Does not throw exception on null
    /// </summary>
    public static bool Is(this XhtmlString xhtmlString)
    {
        return xhtmlString != null && !xhtmlString.IsEmpty;
    }

    /// <summary>
    /// Returns true if the XhtmlString is null or blank or 0 length, else false
    /// 
    /// Note: Does not throw exception on null
    /// </summary>
    public static bool IsNot(this XhtmlString xhtmlString)
    {
        return xhtmlString == null || xhtmlString.IsEmpty;
    }

    public static string Render(this XhtmlString xhtmlString, bool skipWrapperTag = true)
    {
        if (xhtmlString.IsNot()) return "";

        if (xhtmlString.Fragments == null || xhtmlString.Fragments.Count == 0) return "";

        try
        {
            var xhtmlStringHelper = HtmlHelperFactory.Build<XhtmlString>();
            var icontentHelper = HtmlHelperFactory.Build<IContent>();

            var data = xhtmlStringHelper.PropertyFor(x => xhtmlString);
            var rendered = new StringBuilder();
            data = null;
            if (data == null)
                rendered.Append("<div style='color:#e20000;font-size: 14px;line-height:1;'>Exception: block inside xhtmlstring could not be rendered (missing controller? missing view? react-error?):</div>" + xhtmlString.ToHtmlString());
            else
                rendered.Append(data.ToString());

            //Dump.Write(rendered);

            //for (int i = 0; i < xhtmlString.Fragments.Count; i++)
            //{
            //    var fragment = xhtmlString.Fragments[i];
            //    if (fragment == null) continue;

            //    if (fragment is PersonalizedContentFragment personalizedFragment)
            //    {
            //        Dump.Write("Rendering PERZONALIED!");
            //        if (personalizedFragment.ContentGroup.Is())
            //        {
            //            var personalizedFragments = personalizedFragment.Fragments.GetFilteredFragments(PrincipalInfo.CurrentPrincipal);

            //            // Recursive reading of fragments... but for now only the content and string fragments:

            //            foreach (var innerFragment in personalizedFragments)
            //            {
            //                if (innerFragment is ContentFragment personalizedContentFragment)
            //                {
            //                    var content = personalizedContentFragment.GetContent();
            //                    rendered.Append(icontentHelper.PropertyFor(x => content).ToString());
            //                }
            //                else
            //                {
            //                    rendered.Append(innerFragment.GetViewFormat());
            //                }
            //            }
            //        }
            //    }
            //    else if (fragment is ContentFragment contentFragment)
            //    {
            //        Dump.Write("Rendering CONTENTFRAG!!!!!");
            //        //var content = contentFragment.GetContent();
            //        //rendered.Append(icontentHelper.PropertyFor(x => content).ToString());
            //    }
            //    else if (fragment is UrlFragment urlFragment)
            //    {
            //        Dump.Write("Rendering URL!!!!!");
            //        rendered.Append(urlFragment.InternalFormat.ToFriendlyUrl());
            //    }
            //    else if (fragment is StaticFragment staticFragment)
            //    {
            //        //Dump.Write("Rendering STATIC!!!!!");
            //        var staticFragmentUrl = staticFragment.InternalFormat;
            //        if (staticFragmentUrl.Contains("href=\"/EPiServer/CMS/"))
            //        {
            //            rendered.Append("<a class='episerver-link--dead'>Invalid or dead: " + staticFragmentUrl + "</a>");
            //        }
            //        else
            //        {
            //            rendered.Append(staticFragment.InternalFormat);
            //        }
            //    }
            //    else
            //    {
            //        Dump.Write("Rendering GETVIEW!!!!!!!");
            //        var html = fragment.GetViewFormat();
            //        if (html != null)
            //        {
            //            rendered.Append(html);
            //        }
            //    }
            //}

            //var rendered = PropertyExtensions.PropertyFor(helper, x => xhtmlString).ToString();
            // Log.Write(rendered);

            return rendered.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot render XhtmlString to String: " + ex.Message + " " + ex.InnerException?.Message + " " + ex);
        }
    }

    //public class HtmlHelperFactory2
    //{
    //    IHttpContextAccessor _contextAccessor;

    //    public class IndexView : IView
    //    {
    //        public Task RenderAsync(ViewContext context)
    //        {
    //            return Task.CompletedTask;
    //        }

    //        public string Path
    //        {
    //            get
    //            {
    //                return "Index";
    //            }
    //        }
    //    }

    //    public HtmlHelperFactory2()
    //    {
    //        _contextAccessor = Services.Get<IHttpContextAccessor>();
    //    }

    //    public IHtmlHelper<T> Create<T>()
    //    {
    //        var modelMetadataProvider = _contextAccessor.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
    //        var tempDataProvider = _contextAccessor.HttpContext.RequestServices.GetRequiredService<ITempDataProvider>();
    //        var htmlHelper = _contextAccessor.HttpContext.RequestServices.GetRequiredService<IHtmlHelper<T>>();
    //       // using StringWriter writer = new StringWriter();

    //        var viewData = new ViewDataDictionary<T>(modelMetadataProvider, new ModelStateDictionary());

    //        var tempData = new TempDataDictionary(_contextAccessor.HttpContext, tempDataProvider);

    //        var htmlHelperOptions = new HtmlHelperOptions();
    //        var viewContext = new ViewContext(
    //            new ActionContext(_contextAccessor.HttpContext, _contextAccessor.HttpContext.GetRouteData(), new ControllerActionDescriptor()),
    //            new IndexView(),
    //            viewData,
    //            tempData,
    //            TextWriter.Null,
    //            htmlHelperOptions
    //        );

    //        ((IViewContextAware)htmlHelper).Contextualize(viewContext);

    //        return htmlHelper;
    //    }
    //}

    
}