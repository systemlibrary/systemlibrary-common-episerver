using System;
using System.Linq;

using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Web.Mvc.Html;

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
            if (xhtmlString.Fragments.All(fragment => fragment is StaticFragment))
            {
                return xhtmlString.ToHtmlString();
            }

            var xhtmlStringHelper = HtmlHelperFactory.Build<XhtmlString>();

            var data = xhtmlStringHelper.PropertyFor(x => xhtmlString, new { SkipWrapperTag = skipWrapperTag });
            if (data == null)
            {
                Log.Error("Exception: block inside xhtmlstring could not be rendered (missing controller? missing view? react-error? " + xhtmlString.ToHtmlString());

                return "<div style='color:#e20000;font-size: 14px;line-height:1;'>Exception: block inside xhtmlstring could not be rendered (missing controller? missing view? react-error?):</div>" + xhtmlString.ToHtmlString();
            }

            return data.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot render XhtmlString to String: " + ex.Message + " " + ex.InnerException?.Message + " " + ex);
        }
    }
}