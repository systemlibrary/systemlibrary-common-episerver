using EPiServer;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Extensions for Url
/// </summary>
public static class UrlExtensions
{
    /// <summary>
    /// Returns true or false
    /// <para>Does not throw on null</para>
    /// </summary>
    public static bool IsNot(this Url url)
    {
        return url == null || url.OriginalString.IsNot();
    }

    /// <summary>
    /// Returns true or false
    /// <para>Does not throw on null</para>
    /// </summary>
    public static bool Is(this Url url)
    {
        return !url.IsNot();
    }

    /// <summary>
    /// Returns a friendly url behind the content reference, or null if content reference is null or has an invalid ID
    /// <para>Does not throw on null</para>
    /// <para>Never returns null, minimum blank</para>
    /// </summary>
    public static string ToFriendlyUrl(this Url url, bool? convertToAbsolute = null)
    {
        if (IsNot(url)) return "";

        return url.OriginalString.ToFriendlyUrl(convertToAbsolute);
    }
}
