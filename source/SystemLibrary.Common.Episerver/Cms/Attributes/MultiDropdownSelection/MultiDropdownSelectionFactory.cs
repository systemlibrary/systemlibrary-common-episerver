using System;
using System.Collections.Generic;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Cms.Abstract;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public class MultiDropdownSelectionFactory : BaseMultiSelectionFactory, ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        try
        {
            var type = metadata.ModelType;

            var propertyName = metadata.PropertyName;

            var options = GetOptions<MultiDropdownSelectionAttribute>(metadata);

            var selectionType = options.EnumType ?? GetGenericType(type);

            var defaultPropertyIListOfStrings = "epi-cms/contentediting/editors/propertyvaluelist/PropertyValueList";
            if (metadata.ClientEditingClass == defaultPropertyIListOfStrings)
                metadata.ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/MultiDropdownSelection/Script";

            PopulateSelectionItems(items, options, selectionType, metadata);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            items.Add(new SelectItem() { Text = "ERROR: " + ex.Message, Value = "-1" });
        }
        return items;
    }

    
}