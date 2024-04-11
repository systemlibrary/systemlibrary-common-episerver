using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Security;

using SystemLibrary.Common.Episerver.Cms;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class IEnumerableContentReferenceExtensions
{
    public static IEnumerable<T> SelectFiltered<T>(this IEnumerable<ContentReference> contentReferences, bool filterByPublished = false) where T : IContentData
    {
        var items = BaseCms.GetItems<IContent>(contentReferences);

        if (items.IsNot()) yield break;

        var list = items.ToList();

        if (filterByPublished)
        {
            var filter = new FilterContentForVisitor(
              filterPublished: new FilterPublished(PagePublishedStatus.Published),
              filterAccess: new FilterAccess(AccessLevel.Read),
              filterTemplate: new IgnoreTemplateFilter());

            filter.Filter(list);
        }

        foreach (var item in items)
            if (item is T t)
                yield return t;
    }
}
