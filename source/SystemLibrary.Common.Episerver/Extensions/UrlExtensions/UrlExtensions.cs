using EPiServer;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class UrlExtensions
{
    public static bool IsNot(this Url url)
    {
        return url == null || url.OriginalString.IsNot();
    }

    public static bool Is(this Url url)
    {
        return !url.IsNot();
    }

    /// <summary>
    /// Returns a friendly url behind the content reference, or null if content reference is null or has an invalid ID
    /// </summary>
    public static string ToFriendlyUrl(this Url url, bool? convertToAbsolute = null)
    {
        if (IsNot(url)) return "";

        return url.OriginalString.ToFriendlyUrl(convertToAbsolute);
    }
}
