using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Cms;

[EditorDescriptorRegistration(TargetType = typeof(ParentLinkReference))]
public class ParentLinkReferenceEditorDescriptor : EditorDescriptor
{
    public ParentLinkReferenceEditorDescriptor()
    {
        ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/ParentLinkReference/" + nameof(ParentLinkReferenceController.Script);
    }
}
