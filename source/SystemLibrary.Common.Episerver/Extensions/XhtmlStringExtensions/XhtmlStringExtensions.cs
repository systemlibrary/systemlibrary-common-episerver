using System.Text;

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
    /// </summary>
    /// <remarks>
    /// Does not throw exception on null
    /// </remarks>
    public static bool Is(this XhtmlString xhtmlString)
    {
        return xhtmlString != null && !xhtmlString.IsEmpty;
    }

    /// <summary>
    /// Returns true if the XhtmlString is null or blank or 0 length, else false
    /// </summary>
    /// <remarks>
    /// Does not throw on null
    /// </remarks>
    public static bool IsNot(this XhtmlString xhtmlString)
    {
        return xhtmlString == null || xhtmlString.IsEmpty;
    }

    /// <summary>
    /// Render all data, media, blocks, components, inside the XhtmlString
    /// </summary>
    /// <returns>Returns the rendered version of the xhtmlString. Anything from TinyMce HTML text added, to Button Blocks and Accordion Blocks editors have dragged into this field, will be rendered and added to one huge HTML string</returns>
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

            var data = xhtmlStringHelper.PropertyFor(_ => xhtmlString, new { SkipWrapperTag = skipWrapperTag });
            if (data == null)
            {
                const string msg = "Exception: block inside xhtmlstring could not be rendered (missing controller? missing view? react-error? ";

                Log.Error(msg + xhtmlString.ToHtmlString());

                return "<div style='color:#e20000;font-size: 14px;line-height:1;'>" + msg + "</div>" + xhtmlString.ToHtmlString();
            }

            return data.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot render XhtmlString to String: " + ex.Message + " " + ex.InnerException?.Message + " " + ex);
        }
    }

    /// <summary>
    /// Render all data, media, blocks, components, inside the XhtmlString
    /// <para>Use this if you want to add additional data to the string</para>
    /// </summary>
    /// <returns>Returns the rendered version of the xhtmlString. Anything from TinyMce HTML text added, to Button Blocks and Accordion Blocks editors have dragged into this field, will be rendered and added to one huge HTML StringBuilder, ready to be adjusted after</returns>
    public static StringBuilder RenderStringBuilder(this XhtmlString xhtmlString, bool skipWrapperTag = true)
    {
        if (xhtmlString.IsNot()) return new StringBuilder();

        if (xhtmlString.Fragments == null || xhtmlString.Fragments.Count == 0) return new StringBuilder();

        try
        {
            if (xhtmlString.Fragments.All(fragment => fragment is StaticFragment))
            {
                return new StringBuilder(xhtmlString.ToHtmlString());
            }

            var xhtmlStringHelper = HtmlHelperFactory.Build<XhtmlString>();

            var data = xhtmlStringHelper.PropertyFor(_ => xhtmlString, new { SkipWrapperTag = skipWrapperTag, HasContainer = false, HasItemContainer = false });
            if (data == null)
            {
                const string msg = "Exception: block inside xhtmlstring could not be rendered (missing controller? missing view? react-error? ";

                Log.Error(msg + xhtmlString.ToHtmlString());

                return new StringBuilder(
                    "<div style='color:#e20000;font-size: 14px;line-height:1;'>" + msg + "</div>" + xhtmlString.ToHtmlString()
                );
            }

            return new StringBuilder(data.ToString());
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot render XhtmlString to String: " + ex.Message + " " + ex.InnerException?.Message + " " + ex);
        }
    }
}