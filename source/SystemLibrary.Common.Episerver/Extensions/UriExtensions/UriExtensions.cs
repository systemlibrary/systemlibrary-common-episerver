namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Uri extensions
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Returns true or false
    /// <para>Does not throw on null</para>
    /// </summary>
    public static bool IsNot(this Uri uri)
    {
        return uri == null || uri.OriginalString.IsNot();
    }

    /// <summary>
    /// Returns true or false
    /// <para>Does not throw on null</para>
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static bool Is(this Uri uri)
    {
        return !uri.IsNot();
    }

    /// <summary>
    /// Returns the Uri as a Friendly Url
    /// <para>Does not throw on null</para>
    /// </summary>
    public static string ToFriendlyUrl(this Uri uri, bool? convertToAbsolute = null)
    {
        if (uri == null) return null;

        if (uri.OriginalString == null) return null;

        return uri.OriginalString.ToFriendlyUrl(convertToAbsolute);
    }
}
