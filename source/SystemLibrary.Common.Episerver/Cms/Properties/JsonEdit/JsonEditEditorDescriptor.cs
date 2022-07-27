using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Cms;

[EditorDescriptorRegistration(TargetType = typeof(IJsonEdit))]
public class JsonEditEditorDescriptor : EditorDescriptor
{
    public JsonEditEditorDescriptor()
    {
        ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/JsonEdit/" + nameof(JsonEditController.Script);
    }
}
