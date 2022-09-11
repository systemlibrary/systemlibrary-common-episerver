using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Abstract;

public abstract class BaseMultiSelectionFactory
{
    protected Type GetGenericType(Type type)
    {
        foreach (Type @interface in type.GetInterfaces())
        {
            if (@interface.IsGenericType)
                if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return @interface.GetGenericArguments()[0];
        }
        return null;
    }

    protected static Enum AsEnum(string value, Type type)
    {
        return Enum.Parse(type, value) as Enum;
    }

    protected static T GetOptions<T>(ExtendedMetadata metadata) where T : Attribute
    {
        var options = metadata.GetAttribute<T>();

        if (options == null)
            throw new Exception(typeof(T).Name + " can only be used by adding the attribute to the property: " + metadata.PropertyName);

        return (T)options;
    }

    protected (object[] Show, object[] Hide) GetShowHideOptions(Attribute options, string propertyName)
    {
        //NOTE: Hide & Show supports "javascript way", array or 1 item setter
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
            throw new Exception("Show and Hide are both used which is not allowed. Choose between either: A) Show all enum values in the selection by not setting Hide nor Show property in the attribute.B) Show all enum values in selection, but hide a few by setting the Hide property.C) Hide all enum values in selection, but show a few by setting the Show property.Do not set both Hide and Show in the attribute, one of them must be null, or both can be null! Property errored: " + propertyName);

        return (Show, Hide);
    }

    protected IEnumerable<string> KeysFiltered(string[] keys, object[] Show, object[] Hide)
    {
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
                    yield return key;
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
                    yield return key;
            }
            else
                yield return key;
        }
    }

    protected void ShowExpiredItems(bool showExpiredItems, ExtendedMetadata metadata, List<ISelectItem> items) 
    {
        if (!showExpiredItems) return;

        var value = metadata.InitialValue;

        if(value != null && value != "" && value + "" != "0")
        {
            AddExpiredItems(metadata, items);
        }
    }
    
    static void AddExpiredItems(ExtendedMetadata metadata, List<ISelectItem> items)
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