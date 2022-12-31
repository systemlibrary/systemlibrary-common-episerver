using System;
using System.Collections;
using System.Collections.Generic;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Cms.Abstract;
using SystemLibrary.Common.Episerver.FontAwesome;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public class BoxSelectionFactory : BaseMultiSelectionFactory, ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        try
        {
            var options = GetOptions<BoxSelectionAttribute>(metadata);

            var enumType = metadata.ModelType;

            var genericListType = GetGenericListType(enumType);

            var propertyName = metadata.PropertyName;

            if (genericListType == null)
            {
                if (!enumType.IsEnum)
                    enumType = options.EnumType;

                if (enumType == null)
                    throw new Exception("Property " + propertyName + " of type " + metadata.ModelType.Name + " must set the Type of an Enum, in the " + nameof(BoxSelectionAttribute));

                if (!enumType.IsEnum)
                    throw new Exception("Property " + propertyName + " of type " + enumType.Name + " must be an Enum or a String!");

                metadata.EditorConfiguration.Add("isMultiSelect", false);
            }
            else
            {
                if (options.EnumType == null)
                    enumType = genericListType;
                else
                    enumType = options.EnumType;

                if (!enumType.IsEnum)
                    throw new Exception("Property " + propertyName + " has an invalid generic type: " + enumType.Name + ". It must be an Enum or a String");

                metadata.EditorConfiguration.Add("isMultiSelect", true);
            }

            //NOTE: metaData.AdditionalValues is not sent, so have to use EditorConfiguration
            metadata.EditorConfiguration.Add("allowUnselection", options.AllowUnselection);

            (var Show, var Hide) = GetShowHideOptions(options, propertyName);
            
            var keys = Enum.GetNames(enumType);

            foreach (var key in KeysFiltered(keys, Show, Hide))
            {
                items.Add(GetSelectItem(key, enumType, metadata.ContainerType == SystemType.StringType));
            }

            if (options.ShowExpiredItems && metadata.InitialValue != null && metadata.InitialValue != "" && metadata.InitialValue + "" != "0")
            {
                if (genericListType == null)
                {
                    var found = false;
                    foreach (var item in items)
                    {
                        var val = item.Value + "";
                        if (val.Contains("__d_"))
                        {
                            val = val.Split("__d_")[0];
                        }

                        if (val == metadata.InitialValue + "")
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        items.Insert(0, new SelectItem { Text = "Expired: " + metadata.InitialValue, Value = metadata.InitialValue });
                    }
                }
                else
                {
                    var selected = metadata.InitialValue as IList;

                    if (selected != null && selected.Count > 0)
                    {
                        foreach (var selection in selected)
                        {
                            var found = false;
                            var selectedValue = selection + "";
                            foreach (var item in items)
                            {
                                var val = item.Value + "";
                                if (val.Contains("__d_"))
                                {
                                    val = val.Split("__d_")[0];
                                }
                                if (val == selectedValue)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                items.Insert(0, new SelectItem { Text = "Expired: " + selectedValue, Value = selectedValue });
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            items.Add(new SelectItem() { Text = "ERROR: " + ex.Message, Value = "-1" });
        }
        return items;
    }

    static SelectItem GetSelectItem(string key, Type type, bool isStoredAsString)
    {
        string value = null;
        string text = null;

        var e = AsEnum(key, type);

        var enumValue = e.GetEnumValue();

        if (enumValue is FontAwesomeSolid solid)
        {
            value = FontAwesomeLoader.GetFontAwesomeIconRequestUrl(solid);
        }
        else if (enumValue is FontAwesomeRegular regular)
        {
            value = FontAwesomeLoader.GetFontAwesomeIconRequestUrl(regular);
        }
        else if (enumValue is FontAwesomeBrands brands)
        {
            value = FontAwesomeLoader.GetFontAwesomeIconRequestUrl(brands);
        }

        if(value == null)
        {
            if (isStoredAsString)
                value = e.ToValue();
            else
                value = Convert.ToInt32(e) + "__d_" + e.ToValue();

            text = e.ToText();
        }
        else
        {
            if (!isStoredAsString)
                value = Convert.ToInt32(e) + "__d_" + value;
        }

        return new SelectItem { Text = text, Value = value };
    }
}