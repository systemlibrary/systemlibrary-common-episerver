using System;
using System.Text;

using EPiServer.Core;
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
            var xhtmlStringHelper = HtmlHelperFactory.Build<XhtmlString>();
            var icontentHelper = HtmlHelperFactory.Build<IContent>();

            var data = xhtmlStringHelper.PropertyFor(x => xhtmlString, new { SkipWrapperTag = skipWrapperTag });
            var rendered = new StringBuilder();
            data = null;
            if (data == null)
                rendered.Append("<div style='color:#e20000;font-size: 14px;line-height:1;'>Exception: block inside xhtmlstring could not be rendered (missing controller? missing view? react-error?):</div>" + xhtmlString.ToHtmlString());
            else
                rendered.Append(data.ToString());

            return rendered.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot render XhtmlString to String: " + ex.Message + " " + ex.InnerException?.Message + " " + ex);
        }
    }
}