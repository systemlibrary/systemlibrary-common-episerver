using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Cms.Properties;

[EditorDescriptorRegistration(TargetType = typeof(Message))]
public class MessageEditorDescriptor : EditorDescriptor
{
    public MessageEditorDescriptor()
    {
        ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/Message/" + nameof(MessageController.Script);
    }
}
