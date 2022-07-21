﻿using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ContentReferenceExtensions
{
    static IUrlResolver _IUrlResolver;
    static IUrlResolver IUrlResolver =>
        _IUrlResolver != null ? _IUrlResolver :
        (_IUrlResolver = Services.Get<IUrlResolver>());

    /// <summary>
    /// Returns true if the contentReference is null, or have an ID less than 1, else false
    /// </summary>
    /// <param name="contentReference"></param>
    /// <returns></returns>
    public static bool IsNot(this ContentReference contentReference)
    {
        if (contentReference == null || contentReference.ID < 1) return true;

        return false;
    }

    /// <summary>
    /// Returns true if the content reference is not null and has an ID larger than 0, else false
    /// </summary>
    public static bool Is(this ContentReference contentReference)
    {
        return contentReference != null && contentReference.ID > 0;
    }

    /// <summary>
    /// Convert content reference to content data of type T
    /// 
    /// T can be a block, page or media data
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var articlePage = contentReference.To&lt;ArticlePage&gt;();
    /// 
    /// var textBlock = contentReference.To&lt;TextBlock&gt;();
    /// </code>
    /// </example>
    public static T To<T>(this ContentReference contentReference) where T : ContentData
    {
        if (contentReference.IsNot()) return default;

        return BaseCms.Get<T>(contentReference);
    }

    /// <summary>
    /// Returns a friendly url behind the content reference, or null if content reference is null or has an invalid ID
    /// </summary>
    public static string ToFriendlyUrl(this ContentReference contentReference)
    {
        if (contentReference.IsNot()) return null;

        var url = IUrlResolver.GetUrl(contentReference, null, new UrlResolverArguments { ContextMode = ContextMode.Default });

        return url;
    }
}
