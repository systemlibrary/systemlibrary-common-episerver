using System.Linq;

using Castle.Core.Internal;

using EPiServer.Shell.ObjectEditing;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ExtendedMetadataExtensions
{
    public static T GetAttribute<T>(this ExtendedMetadata metadata) where T : System.Attribute
    {
        var attribute = metadata?.Attributes.FirstOrDefault(x => x is T);
        
        if (attribute != null) return attribute as T;

        return metadata.ModelType.GetAttribute<T>();
    }
}
