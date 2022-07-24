using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Castle.Core.Internal;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms;

public class SelectionPickerFactory : ISelectionFactory
{
    Type GetGenericType(Type type)
    {
        foreach (Type @interface in type.GetInterfaces())
        {
            if (@interface.IsGenericType)
                if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return @interface.GetGenericArguments()[0];
        }
        return null;
    }

    static Enum AsEnum(string value, Type type)
    {
        return Enum.Parse(type, value) as Enum;
    }

    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        try
        {
            var options = metadata.GetAttribute<SelectionPickerAttribute>();

            if (options == null)
                throw new Exception(nameof(SelectionPickerAttribute) + " can only be used by adding the attribute to the property: " + metadata.PropertyName);

            var type = metadata.ContainerType;

            var genericType = GetGenericType(type);
            
            if(genericType == null)
            {
                if(!type.IsEnum)
                    type = options.Type;

                if (type == null)
                    throw new Exception("Property " + metadata.PropertyName + " of type " + metadata.ContainerType.Name + " must set the Type of an Enum, in the " + nameof(SelectionPickerAttribute));

                if (!type.IsEnum)
                    throw new Exception("Property " + metadata.PropertyName + " of type " + type.Name + " must be an Enum or a String!");

                metadata.EditorConfiguration.Add("isMultiSelect", false);
            }
            else
            {
                if (options.Type == null)
                    type = genericType;
                else
                    type = options.Type;

                if (!type.IsEnum)
                    throw new Exception("Property " + metadata.PropertyName + " has an invalid generic type: " + type.Name + ". It must be an Enum or a String");

                metadata.EditorConfiguration.Add("isMultiSelect", true);
            }

            //NOTE: EditorConfiguration is used as AdditionalValues is not sent in "simple properties" like string/int...
            metadata.EditorConfiguration.Add("allowUnselection", options.AllowUnselection);

            //NOTE: Hide & Show supports "javascript array or 1 item setter"
            //You can use either:
            //a) Show = Color.White
            //b) Show = new object[] { Color.White, Color.Black }
            object[] Hide = null;
            object[] Show = null;

            var properties = options.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (properties?.Length > 0)
            {
                foreach (var property in properties)
                {
                    if (property.Name == "Hide")
                    {
                        var value = property.GetValue(options);
                        if (value != null)
                        {
                            Hide = value as object[];
                            if (Hide == null)
                                Hide = new object[] { value };
                        }
                    }
                    else if (property.Name == "Show")
                    {
                        var value = property.GetValue(options);
                        if (value != null)
                        {
                            Show = value as object[];
                            if (Show == null)
                                Show = new object[] { value };
                        }
                    }
                }
            }
            if (Show != null && Hide != null)
                throw new Exception("Show and Hide are both used which is not allowed. Choose between either: A) Show all enum values in the selection by not setting Hide nor Show property in the attribute.B) Show all enum values in selection, but hide a few by setting the Hide property.C) Hide all enum values in selection, but show a few by setting the Show property.Do not set both Hide and Show in the attribute, one of them must be null, or both can be null! Property errored: " + metadata.PropertyName);

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
                        if(val.Contains("__d_"))
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

                    if(selected != null && selected.Count > 0)
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
                    //Loop over all items in the selection that are selected, its a List of strings...
                    //all the strings must be within the Enum Names.. else they are expired
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
        var e = AsEnum(key, type);

        if (storedAsString)
            return new SelectItem { Text = e.ToText(), Value = e.ToValue() };
        else
            return new SelectItem { Text = e.ToText(), Value = Convert.ToInt32(e).ToString() + "__d_" + e.ToValue() };
    }
}