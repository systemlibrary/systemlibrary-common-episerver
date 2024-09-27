using EPiServer;
using EPiServer.Core;
using EPiServer.Security;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class IContentDataExtensions
{
    public static bool IsDeleted(this IContentData contentData)
    {
        if (contentData == null ||
            (contentData is IContent content && content.IsDeleted))
            return true;

        var type = contentData.GetOriginalType();

        return type.Name == "BlockData" || type.Name == "PageData" || type.Name == "MediaData";
    }

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
