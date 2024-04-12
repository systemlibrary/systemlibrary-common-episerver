using System.Collections.Generic;
using System.Linq;
using System.Text;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Security;
using EPiServer.Web.Mvc.Html;

using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SystemLibrary.Common.Episerver.Cms;
using SystemLibrary.Common.Net.Extensions;

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

    /// <summary>
    /// Render all fragments in the 'Content Area' to a string.
    /// 
    /// Returns a string with all HTML generated from the 'Content Area' based on current visitors access
    /// </summary>
    public static string Render(this ContentArea contentArea)
    {
        return RenderStringBuilder(contentArea).ToString();
    }

    /// <summary>
    /// Render all fragments in the 'Content Area' to a StringBuilder.
    /// 
    /// Returns a StringBuilder with all HTML generated from the 'Content Area' based on current visitors access
    /// </summary>
    public static StringBuilder RenderStringBuilder(this ContentArea contentArea)
    {
        if (contentArea.IsNot()) return new StringBuilder("");

        var filteredItems = contentArea.FilteredItems;

        var rendered = new StringBuilder("", capacity: 512);

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
                var media = item.ContentLink.To<MediaData>();
                if (media != null)
                {
                    if (media.Name == "MediaData") continue;

                    if (media.IsDeleted) continue;

                    if (media.IsPublished())
                        rendered.Append(iContentHtmlHelper.PropertyFor(x => media).ToString());
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
        }

        return rendered;
    }

    /// <summary>
    /// Select ContentData  from 'ContentArea' based on what the current visitor has access to.
    /// 
    /// Optional: only return content that is published
    /// </summary>
    /// <returns>Returns an IEnumerable of IContentData</returns>
    public static IEnumerable<T> SelectFiltered<T>(this ContentArea contentArea, bool filterByPublished = false) where T : IContentData
    {
        if (contentArea.IsNot()) yield break;

        var references = contentArea.FilteredItems.Select(item => item.ContentLink);

        var enumerable = references.SelectFiltered<T>(filterByPublished);

        foreach(var item in enumerable) yield return item;
    }
}
