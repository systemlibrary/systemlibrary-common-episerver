using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Common page data extensions
/// </summary>
public static class PageDataExtensions
{
    /// <summary>
    /// Returns true if 'pageData' is null or if the content link of it is null
    /// </summary>
    public static bool IsNot(this PageData pageData)
    {
        if (pageData == null || pageData.ContentLink == null) return true;

        return false;
    }

    /// <summary>
    /// Returns true if 'pageData' is not null and content link on pageData exists
    /// </summary>
    public static bool Is(this PageData pageData)
    {
        return !IsNot(pageData);
    }

    /// <summary>
    /// Returns friendly url or null if pageData is null or the content link to the pageData is null
    /// </summary>
    public static string ToFriendlyUrl(this PageData pageData)
    {
        if (pageData.Is())
            return pageData.ContentLink.ToFriendlyUrl();

        return null;
    }
}