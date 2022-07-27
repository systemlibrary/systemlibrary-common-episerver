using System;
using System.Collections.Generic;

using Castle.Core.Internal;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public class JsonEditFactory : ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        try
        {
            var options = metadata.GetAttribute<JsonEditAttribute>();

            if (options == null)
                throw new Exception(nameof(JsonEditAttribute) + " can only be used by adding the attribute to the property: " + metadata.PropertyName);

            var type = metadata.ContainerType;

            metadata.EditorConfiguration.Add("typeName", options.Type.Name);
            metadata.EditorConfiguration.Add("jsonSchema", "");
            metadata.EditorConfiguration.Add("jsonEditorUrl", "/SystemLibrary/Common/Episerver/UiHint/JsonEdit/" + nameof(JsonEditController.Editor));
        }
        catch (Exception ex)
        {
            items.Add(new SelectItem() { Text = "ERROR: " + ex.Message, Value = "-1" });
        }
        return items;
    }
}