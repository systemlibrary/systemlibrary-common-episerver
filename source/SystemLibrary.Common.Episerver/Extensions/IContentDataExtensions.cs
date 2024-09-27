using EPiServer;
using EPiServer.Core;
using EPiServer.Security;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class IContentDataExtensions
{
    /// <summary>
    /// Returns the content reference link of IContentData if it implements IContent
    /// <para>Returns null other wise</para>
    /// </summary>
    public static ContentReference ContentLink(this IContentData contentData)
    {
        if (contentData == null) return null;

        if (contentData is IContent content)
            return content.ContentLink;

        return null;
    }

    /// <summary>
    /// Returns true if content is deleted or if the code of the content is deleted, else false
    /// </summary>
    public static bool IsDeleted(this IContentData contentData)
    {
        if (contentData == null ||
            (contentData is IContent content && content.IsDeleted))
            return true;

        var type = contentData.GetOriginalType();

        return type.Name == "BlockData" || type.Name == "PageData" || type.Name == "MediaData";
    }

    /// <summary>
    /// Returns true if content is displayable in regards to permission, published and not deleted
    /// </summary>
    public static bool IsDisplayable(this IContentData contentData)
    {
        if (contentData.IsDeleted()) return false;

        if (contentData is IVersionable versionable)
        {
            if (versionable.Status != VersionStatus.Published)
            {
                return false;
            }
        }

        if (contentData is IContent icontent)
        {
            return icontent.QueryDistinctAccess(AccessLevel.Read);
        }

        return true;
    }
}
