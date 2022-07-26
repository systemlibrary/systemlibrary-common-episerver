using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Cms;

[EditorDescriptorRegistration(TargetType = typeof(Message))]
public class MessageEditorDescriptor : EditorDescriptor
{
    public MessageEditorDescriptor()
    {
        ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/Message/" + nameof(MessageController.Script);
    }
}
