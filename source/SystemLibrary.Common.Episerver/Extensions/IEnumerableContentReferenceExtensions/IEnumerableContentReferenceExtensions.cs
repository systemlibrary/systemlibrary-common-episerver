using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Security;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class IEnumerableContentReferenceExtensions
{
    /// <summary>
    /// Convert content references To T
    /// <para>Item must be castable to T to be yield</para>
    /// </summary>
    /// <param name="filterByDisplayable">Set to True to force filtering on Published Content, also filters away deleted content and deleted content types and user must have read access</param>
    /// <returns>Returns an IEnumerable of T</returns>
    public static IEnumerable<T> To<T>(this IEnumerable<ContentReference> contentReferences, bool filterByDisplayable = false) where T : IContentData
    {
        if (contentReferences.IsNot()) yield break;

        var items = BaseCms.ContentRepository.GetItems(contentReferences, new LoaderOptions());

        if (items.IsNot()) yield break;

        if (filterByDisplayable)
        {
            var list = items.ToList();

            var filter = new FilterContentForVisitor(
              filterPublished: new FilterPublished(PagePublishedStatus.Published),
              filterAccess: new FilterAccess(AccessLevel.Read),
              filterTemplate: new IgnoreTemplateFilter());

            filter.Filter(list);

            foreach (var item in list)
            {
                if (item is T t && t is IContent icontent && !icontent.IsDeleted)
                {
                    var type = item.GetOriginalType();

                    // A deleted block or page, if still part of the ContentReferences will be returned as their default type name, so filter it out
                    if (type.Name != "BlockData" && type.Name != "PageData")
                        yield return t;
                }
            }
        }
        else
        {
            foreach (var item in items)
            {
                if (item is T t)
                    yield return t;
            }
        }
    }
}
