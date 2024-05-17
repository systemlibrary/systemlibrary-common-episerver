using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Security;

using SystemLibrary.Common.Episerver;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class IEnumerableContentReferenceExtensions
{
    /// <summary>
    /// Select ContentData from 'ContentArea' as T
    /// 
    /// Optional: force filterByPublished, even if current visitor have access to view unpublished content
    /// </summary>
    /// <returns>Returns an IEnumerable of ContentData</returns>
    public static IEnumerable<T> To<T>(this IEnumerable<ContentReference> contentReferences, bool filterByPublished = false) where T : IContent
    {
        if (contentReferences.IsNot()) yield break;

        var items = BaseCms.ContentRepository.GetItems(contentReferences, new LoaderOptions());

        if (items.IsNot()) yield break;

        if (filterByPublished)
        {
            var list = items.ToList();

            var filter = new FilterContentForVisitor(
              filterPublished: new FilterPublished(PagePublishedStatus.Published),
              filterAccess: new FilterAccess(AccessLevel.Read),
              filterTemplate: new IgnoreTemplateFilter());

            filter.Filter(list);

            foreach (var item in list)
                if (item is T t && !t.IsDeleted)
                    yield return t;
        }
        else
        {
            foreach (var item in items)
                if (item is T t)
                    yield return t;
        }
    }
}
