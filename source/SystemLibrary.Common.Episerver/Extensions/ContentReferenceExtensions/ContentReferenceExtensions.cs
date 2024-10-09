using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Common content reference extensions
/// </summary>
public static class ContentReferenceExtensions
{
    static IUrlResolver _IUrlResolver;
    static IUrlResolver IUrlResolver => _IUrlResolver ??= Services.Get<IUrlResolver>();

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
    /// <para>T can be a block, page or media data</para>
    /// Returns content or null if not found
    /// </summary>
    /// <remarks>
    /// filterByDisplayable: content cannot be deleted, it must be published, current user must have read access and code of the content type must exist
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var articlePage = contentReference.To&lt;ArticlePage&gt;();
    /// 
    /// var textBlock = contentReference.To&lt;TextBlock&gt;();
    /// </code>
    /// </example>
    public static T To<T>(this ContentReference contentReference, bool filterByDisplayable = true) where T : IContentData
    {
        if (contentReference.IsNot()) return default;

        BaseCms.ContentRepository.TryGet(contentReference, out T content);

        if (!filterByDisplayable) return content;

        if (filterByDisplayable && content.IsDisplayable())
        {
            return content;
        }
        return default;
    }

    /// <summary>
    /// Returns a friendly url behind the content reference, or null if content reference is null or has an invalid ID
    /// </summary>
    public static string ToFriendlyUrl(this ContentReference contentReference, bool? convertToAbsolute = null)
    {
        if (contentReference.IsNot()) return null;

        var url = IUrlResolver?.GetUrl(contentReference, null, new UrlResolverArguments { ContextMode = ContextMode.Default });

        return url.ToFriendlyUrl(convertToAbsolute);
    }
}
