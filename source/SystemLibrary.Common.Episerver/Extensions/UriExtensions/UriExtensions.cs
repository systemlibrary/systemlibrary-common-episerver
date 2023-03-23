using System;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class UriExtensions
{
    public static bool IsNot(this Uri uri)
    {
        return uri == null || uri.OriginalString.IsNot();
    }

    public static bool Is(this Uri uri)
    {
        return !uri.IsNot();
    }

    public static string ToFriendlyUrl(this Uri uri, bool? convertToAbsolute  =null)
    {
        if (uri == null) return null;

        if (uri.OriginalString == null) return null;

        return uri.OriginalString.ToFriendlyUrl(convertToAbsolute);
    }
}
