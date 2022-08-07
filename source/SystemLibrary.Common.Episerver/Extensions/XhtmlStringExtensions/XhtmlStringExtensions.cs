using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Common xhtmlstring extensions
/// </summary>
public static class XhtmlStringExtensions
{
    /// <summary>
    /// Returns true if the XhtmlString contains some value, else false
    /// 
    /// Note: Does not throw exception on null
    /// </summary>
    public static bool Is(this XhtmlString xhtmlString)
    {
        return xhtmlString != null && !xhtmlString.IsEmpty;
    }

    /// <summary>
    /// Returns true if the XhtmlString is null or blank or 0 length, else false
    /// 
    /// Note: Does not throw exception on null
    /// </summary>
    public static bool IsNot(this XhtmlString xhtmlString)
    {
        return xhtmlString == null || xhtmlString.IsEmpty;
    }
}