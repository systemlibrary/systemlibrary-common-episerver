using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions
{
    public static class ContentAreaExtensions
    {
        public static bool IsNot(this ContentArea contentArea)
        {
            return contentArea == null || contentArea.Items == null || contentArea.Items.Count == 0;
        }

        public static bool Is(this ContentArea contentArea)
        {
            return contentArea != null && contentArea.Items != null && contentArea.Items.Count > 0;
        }
    }
}
