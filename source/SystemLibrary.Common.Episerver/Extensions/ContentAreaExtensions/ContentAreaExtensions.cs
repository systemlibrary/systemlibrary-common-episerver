using System.Linq;
using System.Text;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Web.Mvc.Html;

using static SystemLibrary.Common.Episerver.Extensions.XhtmlStringExtensions;

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
        foreach (var item in filteredItems)
        {
            var block = item.ContentLink.To<BlockData>();
            if (block != null)
            {
                var type = block.GetOriginalType();

                // Deleted pages and blocks in a contentarea returns the base type name "BlockData" or "PageData"
                if (type.Name != "BlockData" && type.Name != "PageData")
                {
                    var blockDataHelper = HtmlHelperFactory.Build<BlockData>();
                    rendered.Append(blockDataHelper.PropertyFor(x => block).ToString());
                }
            }
            else
            {
                var page = item.ContentLink.To<PageData>();
                if(page != null)
                {
                    if (page.IsPublished())
                    {
                        var pageDataHelper = HtmlHelperFactory.Build<PageData>();
                        rendered.Append(pageDataHelper.PropertyFor(x => page).ToString());
                    }
                }
            }
        }

        return rendered.ToString();
    }
}
