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

            var type = metadata.ContainerType;

            var genericType = GetGenericType(type);

            if (genericType == null)
            {
                if (!type.IsEnum)
                    type = options.EnumType;

                if (type == null)
                    throw new Exception("Property " + metadata.PropertyName + " of type " + metadata.ContainerType.Name + " must set the Type of an Enum, in the " + nameof(BoxSelectionAttribute));

                if (!type.IsEnum)
                    throw new Exception("Property " + metadata.PropertyName + " of type " + type.Name + " must be an Enum or a String!");

                metadata.EditorConfiguration.Add("isMultiSelect", false);
            }
            else
            {
                if (options.EnumType == null)
                    type = genericType;
                else
                    type = options.EnumType;

                if (!type.IsEnum)
                    throw new Exception("Property " + metadata.PropertyName + " has an invalid generic type: " + type.Name + ". It must be an Enum or a String");

                metadata.EditorConfiguration.Add("isMultiSelect", true);
            }

            //NOTE: EditorConfiguration is used as AdditionalValues is not sent in "simple properties" like string/int...
            metadata.EditorConfiguration.Add("allowUnselection", options.AllowUnselection);

            (var Show, var Hide) = GetShowHideOptions(options, metadata.PropertyName);
            
            var keys = Enum.GetNames(type);

            foreach (var key in keys)
            {
                if (Hide != null)
                {
                    var skip = false;
                    foreach (var hide in Hide)
                    {
                        if (hide.ToString() == key)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (!skip)
                        items.Add(GetSelectItem(key, type, metadata.ContainerType == SystemType.StringType));
                }
                else if (Show != null)
                {
                    var skip = true;
                    foreach (var show in Show)
                    {
                        if (show.ToString() == key)
                        {
                            skip = false;
                            break;
                        }
                    }

                    if (!skip)
                        items.Add(GetSelectItem(key, type, metadata.ContainerType == SystemType.StringType));
                }
                else
                    items.Add(GetSelectItem(key, type, metadata.ContainerType == SystemType.StringType));
            }

            if (options.ShowExpiredItems && metadata.InitialValue != null && metadata.InitialValue != "" && metadata.InitialValue + "" != "0")
            {
                if (genericType == null)
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

    static SelectItem GetSelectItem(string key, Type type, bool storedAsString)
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
            if (storedAsString)
                value = e.ToValue();
            else
                value = Convert.ToInt32(e) + "__d_" + e.ToValue();

            text = e.ToText();
        }
        else
        {
            if (!storedAsString)
                value = Convert.ToInt32(e) + "__d_" + value;
        }

        return new SelectItem { Text = text, Value = value };
    }
}