using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions
{
    public static class ContentAreaExtensions
    {
        /// <summary>
        /// Returns true if the content area is null or have no items added to it, else false
        /// </summary>
        /// <param name="contentArea"></param>
        /// <returns></returns>
        public static bool IsNot(this ContentArea contentArea)
        {
            return contentArea == null || contentArea.Items == null || contentArea.Items.Count == 0;
        }

        /// <summary>
        /// Returns true if the content area is not null and has at least 1 item in it, else false
        /// </summary>
        public static bool Is(this ContentArea contentArea)
        {
            return contentArea != null && contentArea.Items != null && contentArea.Items.Count > 0;
        }
    }
}
