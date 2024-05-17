using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Properties;

[EditorDescriptorRegistration(TargetType = typeof(Message))]
public class MessageEditorDescriptor : EditorDescriptor
{
    public MessageEditorDescriptor()
    {
        ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/Message/" + nameof(MessageController.Script);
    }
}
