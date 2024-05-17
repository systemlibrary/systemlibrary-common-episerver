using System;
using System.Collections.Generic;

using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

using SystemLibrary.Common.Episerver.Attributes;

namespace SystemLibrary.Common.Episerver.Properties;

[EditorDescriptorRegistration(TargetType = typeof(ParentLinkReference))]
public class ParentLinkReferenceEditorDescriptor : EditorDescriptor
{
    public ParentLinkReferenceEditorDescriptor()
    {
        ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/ParentLinkReference/" + nameof(ParentLinkReferenceController.Script);
    }

    public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        base.ModifyMetadata(metadata, attributes);
        metadata.SelectionFactoryType = typeof(ParentLinkReferenceFactory);
    }
}
