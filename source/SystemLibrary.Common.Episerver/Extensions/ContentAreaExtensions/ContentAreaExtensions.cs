﻿using System.Text;

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

                if (type.Name == "BlockData")
                {
                    Debug.Log("Skipping block data, most likely deleted: " + type.Name + " " + (block as IContent)?.IsDeleted);
                    continue;
                }

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
    /// Convert items in ContentArea To T
    /// <para>Filters on current user rights, personalization</para>
    /// Item must be castable to T to be yield
    /// </summary>
    /// <remarks>
    /// You can use 'SelectFiltered' to ReactServerSideRender as 'additional props' to bypass the way ContentArea are rendered, which in return will convert all properties in all content of the area to 'dictionary', which can easily be used as Props
    /// </remarks>
    /// <param name="filterByDisplayable">False filters by permission, published and personalization. True also checks that the content model exists in code</param>
    /// <returns>Returns an IEnumerable of T</returns>
    public static IEnumerable<T> SelectFiltered<T>(this ContentArea contentArea, bool filterByDisplayable = false) where T : IContentData
    {
        if (contentArea.IsNot()) yield break;

        var references = contentArea.FilteredItems.Select(item => item.ContentLink);

        foreach (var item in references.To<T>(filterByDisplayable))
            yield return item;
    }
}
