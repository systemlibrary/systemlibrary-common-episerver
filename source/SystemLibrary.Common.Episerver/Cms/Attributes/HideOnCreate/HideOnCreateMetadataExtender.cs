using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;

namespace SystemLibrary.Common.Episerver.Attributes;

internal class HideOnCreateMetadataExtender : IMetadataExtender
{
    public void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        // When content is being created the content link is 0
        if (metadata.Model is IContent data && data.ContentLink.ID == 0)
        {
            foreach (var modelMetadata in metadata.Properties)
            {
                var property = (ExtendedMetadata)modelMetadata;
                // The content is being created, so set required = false
                if (property.Attributes.OfType<HidePropertyOnCreateAttribute>().Any())
                {
                    property.IsRequired = false;
                }
            }
        }
    }
}
