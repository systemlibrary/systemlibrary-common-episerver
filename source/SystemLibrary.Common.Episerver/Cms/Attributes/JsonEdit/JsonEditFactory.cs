using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Castle.Core.Internal;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public class JsonEditFactory : ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        try
        {
            var options = metadata.GetAttribute<JsonEditAttribute>();

            if (options?.Type == null)
                throw new Exception(nameof(JsonEditAttribute) + " can only be used by adding the attribute to the property: " + metadata.PropertyName + ", and you must set the Type property of the attribute");

            var type = metadata.ContainerType;

            if(type != SystemType.StringType)
                throw new Exception("Cannot add 'JsonEdit' attribute on property of type " + type.Name + ". Change it to a string");

            var title = options.Type.Name;
            var displayName = options.Type.GetAttribute<DisplayAttribute>();
            if (displayName?.Name.Is() == true)
                title = displayName.Name;

            if (options.Title.Is())
                title = options.Title;

            metadata.EditorConfiguration.Add("jsonEditTitle", title);
            metadata.EditorConfiguration.Add("jsonEditProperties", JsonEditPropertiesLoader.GetPropertySchema(options.Type));
            metadata.EditorConfiguration.Add("jsonEditSortByPropertyName1", options.SortByPropertyName1);
            metadata.EditorConfiguration.Add("jsonEditSortByPropertyName2", options.SortByPropertyName2);
            metadata.EditorConfiguration.Add("jsonEditorUrl", "/SystemLibrary/Common/Episerver/UiHint/JsonEdit/" + nameof(JsonEditController.EditorHtml));
        }
        catch (Exception ex)
        {
            items.Add(new SelectItem() { Text = "ERROR: " + ex.Message, Value = "-1" });
        }
        return items;
    }
}