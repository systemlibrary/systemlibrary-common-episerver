using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class IntExtensions
{
    /// <summary>
    /// Convert an ID to pagedata, blockdata or mediadata in regards to filtered by published and permissions access for current user
    /// <para>filterByDisplayable to:</para>
    /// <para>Not return deleted content that lives in Waste Basket</para>
    /// <para>Not return content if the code for the contentdata has been deleted</para>
    /// <para>Content must be published and user must have read access</para>
    /// </summary>
    public static T ToContent<T>(this int id, bool filterByDisplayable = true) where T : ContentData
    {
        if (id < 1) return default;

        var contentReference = new ContentReference(id);

        return contentReference.To<T>(filterByDisplayable);
    }
}
