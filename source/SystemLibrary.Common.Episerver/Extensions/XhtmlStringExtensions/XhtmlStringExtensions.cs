using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class XhtmlStringExtensions
{
    public static bool Is(this XhtmlString xhtmlString)
    {
        return xhtmlString != null && !xhtmlString.IsEmpty;
    }

    public static bool IsNot(this XhtmlString xhtmlString)
    {
        return xhtmlString == null || xhtmlString.IsEmpty;
    }
}