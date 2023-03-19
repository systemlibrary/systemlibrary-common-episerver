using System.Linq;
using System.Text;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Web.Mvc.Html;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Common content area extensions
/// </summary>
public static class ContentAreaExtensions
{
    /// <summary>
    /// Returns true if the content area has at 0 items that can be displayed for current user (filtered), else false
    /// </summary>
    public static bool IsNot(this ContentArea contentArea)
    {
        return contentArea == null || contentArea.Items == null || contentArea.Items.Count == 0 ||
            contentArea.FilteredItems?.Count() < 1;
    }

    /// <summary>
    /// Returns true if the content area has at least 1 item that can be displayed for current user (filtered), else false
    /// </summary>
    public static bool Is(this ContentArea contentArea)
    {
        return contentArea != null && contentArea.Items != null && contentArea.Items.Count > 0
            && contentArea.FilteredItems?.Count() > 0;
    }

    public static string Render(this ContentArea contentArea)
    {
        if (contentArea.IsNot()) return "";

        var filteredItems = contentArea.FilteredItems;

        var rendered = new StringBuilder("");

        var iContentHtmlHelper = HtmlHelperFactory.Build<IContent>();

        foreach (var item in filteredItems)
        {
            // Deleted pages and blocks in contentarea returns type BlockData or PageData
            var block = item.ContentLink.To<BlockData>();
            if (block != null)
            {
                var type = block.GetOriginalType();
                if (type.Name == "BlockData") continue;
                
                rendered.Append(iContentHtmlHelper.PropertyFor(x => block).ToString());
            }
            else
            {
                var page = item.ContentLink.To<PageData>();

                if (page == null) continue;

                var type = page.GetOriginalType();
                if (type.Name == "PageData") continue;

                if (page.IsPublished())
                {
                    rendered.Append(iContentHtmlHelper.PropertyFor(x => page).ToString());
                }
            }
        }

        return rendered.ToString();
    }
}
