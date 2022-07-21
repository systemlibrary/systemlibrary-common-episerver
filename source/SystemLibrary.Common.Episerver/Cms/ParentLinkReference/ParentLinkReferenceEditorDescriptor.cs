using EPiServer.Core;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Cms;

//[EditorDescriptorRegistration(TargetType = typeof(ParentLinkReference), UIHint = ParentLinkReferenceUiHint.UiHint)]
[EditorDescriptorRegistration(TargetType = typeof(ParentLinkReference))]
public class ParentLinkReferenceEditorDescriptor : EditorDescriptor
{
    public ParentLinkReferenceEditorDescriptor()
    {
        ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/ParentLinkReference/" + nameof(ParentLinkReferenceController.Script);
    }
}

[EditorDescriptorRegistration(TargetType = typeof(PageData))]
public class PageDataEditorDescriptor : EditorDescriptor
{
    public PageDataEditorDescriptor()
    {
        Dump.Write("PageData!");
    }
}