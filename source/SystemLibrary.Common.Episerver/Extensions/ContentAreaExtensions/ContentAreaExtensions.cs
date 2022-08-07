using System.Linq;

using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Common content area extensions
/// </summary>
public static class ContentAreaExtensions
{
    /// <summary>
    /// Returns true if the content area has at 0 items that can be displayed for current user (filtered), else false
    /// </summary>
    public static bool IsNot(this ContentArea contentArea)
    {
        return contentArea == null || contentArea.Items == null || contentArea.Items.Count == 0 ||
            contentArea.FilteredItems?.Count() < 1;
    }

    /// <summary>
    /// Returns true if the content area has at least 1 item that can be displayed for current user (filtered), else false
    /// </summary>
    public static bool Is(this ContentArea contentArea)
    {
        return contentArea != null && contentArea.Items != null && contentArea.Items.Count > 0
            && contentArea.FilteredItems?.Count() > 0;
    }
}
