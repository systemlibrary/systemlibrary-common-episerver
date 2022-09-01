using System;
using System.Collections;
using System.Collections.Generic;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Cms.Abstract;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public class MultiDropdownSelectionFactory : BaseMultiSelectionFactory, ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        try
        {
            var options = GetOptions<MultiDropdownSelectionAttribute>(metadata);

            var type = metadata.ContainerType;

            var propertyName = metadata.PropertyName;

            var genericType = GetGenericType(type);

            if (genericType == null)
                throw new Exception("Property " + propertyName + " is of wrong type, must be an IList<string>");

            var enumType = options.EnumType;

            if (enumType != null && !enumType.IsEnum)
                throw new Exception("Property " + propertyName + " has an invalid 'EnumType', the type must be an Enum");

            (var Show, var Hide) = GetShowHideOptions(options, propertyName);

            var value = metadata.InitialValue;

            if (enumType != null)
            {
                var keys = Enum.GetNames(enumType);

                foreach (var key in KeysFiltered(keys, Show, Hide))
                    items.Add(GetSelectItemFromEnumType(key, enumType));

                ShowExpiredItems(options.ShowExpiredItems, metadata, items);
            }
            else
            {
                //Its a comma separated list of data, that was manually entered by a "input field" where Editors can write whatever they want? Or???
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex);
            items.Add(new SelectItem() { Text = "ERROR: " + ex.Message, Value = "-1" });
        }
        return items;
    }

    static SelectItem GetSelectItemFromEnumType(string key, Type type)
    {
        var e = AsEnum(key, type);

        var value = e.ToString();
        var text = e.ToText();

        //Does not support showing "images" through "EnumText", but can be expanded to do so for visual
        return new SelectItem { Text = text, Value = value };
    }
}