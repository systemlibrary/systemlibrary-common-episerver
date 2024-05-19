using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using EPiServer;
using EPiServer.Cms.Shell.UI.UIDescriptors.Internal;
using EPiServer.Shell.ObjectEditing;

using MailKit;

using SystemLibrary.Common.Episerver.Abstract;
using SystemLibrary.Common.Episerver.FontAwesome;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

using WeCantSpell.Hunspell;

namespace SystemLibrary.Common.Episerver.Attributes;

public class BoxSelectionFactory : BaseMultiSelectionFactory, ISelectionFactory
{
    const string IntDelimiter = "__d_";

    enum StoreType
    {
        String,
        Int,
        Enum,
        ListString,
        ListInt,
        ListEnum
    }

    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var boxes = new List<ISelectItem>();

        var propertyName = metadata.PropertyName;

        if (metadata.EditorConfiguration.Count > 2)
        {
            return Enumerable.Empty<ISelectItem>();
        }
        try
        {
            var options = GetOptions<BoxSelectionAttribute>(metadata);

            var modelType = metadata.ModelType;

            var genericType = modelType.GetFirstGenericType();

            var selectEnumType = options.EnumType;

            if (selectEnumType == null)
            {
                selectEnumType = modelType;
                if (selectEnumType?.IsEnum != true)
                {
                    selectEnumType = genericType;
                }
            }

            if (selectEnumType?.IsEnum != true)
            {
                throw new Exception(propertyName + " must have an EnumType specified, either directly as the property type, or through the attribute's arguments. Type is not enum: " + selectEnumType?.Name);
            }

            Type storeEnum = null;
            if (modelType.IsEnum)
                storeEnum = modelType;
            else if (genericType?.IsEnum == true)
                storeEnum = genericType;

            Type genericTypeDef = null;

            if (modelType.IsGenericType)
                genericTypeDef = modelType.GetGenericTypeDefinition();

            var isMultiSelect = modelType.IsListOrArray() || genericTypeDef == typeof(IList<>);

            metadata.EditorConfiguration.Add(nameof(isMultiSelect), isMultiSelect);

            var storeAsEnum = storeEnum?.IsEnum == true;
            metadata.EditorConfiguration.Add(nameof(storeAsEnum), storeAsEnum);

            metadata.EditorConfiguration.Add("allowUnselection", options.AllowUnselection);

            (var Show, var Hide, var ShowExpiredItems) = GetAttributeOptions(options);

            var keys = Enum.GetNames(selectEnumType);

            List<string> keysStorable = null;
            if (storeEnum?.IsEnum == true)
            {
                var names = Enum.GetNames(storeEnum).ToList();

                keysStorable = new List<string>();

                foreach (var key in names)
                {
                    var e = AsEnum(key, storeEnum);
                    keysStorable.Add(key);
                    keysStorable.Add(e.ToValue());
                    keysStorable.Add(e.ToText());
                    keysStorable.Add(((int)(object)e).ToString());
                }
                keysStorable = keysStorable.Distinct().ToList();
            }

            var storeAsString = modelType == SystemType.StringType || modelType.GetFirstGenericType() == SystemType.StringType;

            foreach (var selectKey in KeysFiltered(keys, Show, Hide))
            {
                if (KeyEligibleForStorage(selectKey, selectEnumType, keysStorable, storeEnum))
                {
                    boxes.Add(GetSelectItem(selectKey, selectEnumType, storeAsString));
                }
            }

            var hasSelectedValue = metadata.InitialValue != null && metadata.InitialValue != "" && metadata.InitialValue + "" != "0";

            if (options.ShowExpiredItems && hasSelectedValue)
            {
                if (!isMultiSelect)
                {
                    var found = false;
                    var selectedValue = metadata.InitialValue + "";
                    foreach (var box in boxes)
                    {
                        var boxValue = box.Value + "";
                        if (boxValue.Contains(IntDelimiter))
                        {
                            boxValue = boxValue.Split(IntDelimiter)[0];
                        }

                        if (boxValue == selectedValue)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (selectedValue == "" || selectedValue == "null" || selectedValue == "0")
                    {
                        found = true;
                    }

                    if (!found && storeAsEnum)
                    {
                        // Check if the selected value actually is the "First Enum" as then, it is selected and no need to add it as "expired"
                        var e = AsEnum(selectedValue, storeEnum);
                        var i = (int)(object)e;
                        var v = e.ToValue();
                        if (i == 0 || v == null || v == "" || v == "0")
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        boxes.Insert(0, new SelectItem { Text = "Expired: " + metadata.InitialValue, Value = metadata.InitialValue });
                    }
                }
                else
                {
                    var selection = metadata.InitialValue as IList;

                    if (selection?.Count > 0)
                    {
                        // List of Enum, int or strings
                        foreach (var selected in selection)
                        {
                            var found = false;

                            var selectedValue = selected + "";

                            if (storeAsEnum)
                            {
                                foreach (var box in boxes)
                                {
                                    if ((box.Value + "").Contains(selectedValue) || box.Text.Contains(selectedValue))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var box in boxes)
                                {
                                    var boxValue = box.Value + "";
                                    if (boxValue.Contains(IntDelimiter))
                                    {
                                        boxValue = boxValue.Split(IntDelimiter)[0];
                                    }
                                    if (boxValue == selectedValue)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }

                            if(!found && storeAsEnum)
                            {
                                var e = AsEnum(selectedValue, storeEnum);
                                var i = (int)(object)e;
                                var v = e.ToValue();
                                if (i == 0 || v == null || v == "" || v == "0")
                                {
                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                boxes.Insert(0, new SelectItem { Text = "Expired: " + selectedValue, Value = selectedValue });
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(propertyName + ": " + ex);

            boxes.Add(new SelectItem() { Text = "ERROR: " + ex.Message, Value = "-99999" });
        }
        return boxes;
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

        if (value != null)
        {
            // FontAwesome url is sent with the value, but value should be stored as Int, so pass both as "value"
            if (!isStoredAsString)
                value = Convert.ToInt32(e) + IntDelimiter + value;
        }

        text = e.ToText();

        if (value == null)
        {
            if (isStoredAsString)
                value = e.ToValue();
            else
                value = Convert.ToInt32(e) + IntDelimiter + e.ToValue();
            // Value is stored as Int, so pass both Int and Value to create proper css classes
        }

        return new SelectItem { Text = text, Value = value };
    }

    static bool KeyEligibleForStorage(string selectKey, Type selectEnum, List<string> storableValues, Type storeEnum)
    {
        if (storeEnum?.IsEnum != true) return true;

        if (selectEnum == storeEnum) return true;

        if (storableValues.Contains(selectKey)) return true;

        var e = AsEnum(selectKey, selectEnum);
        var v = e.ToValue();
        var t = e.ToText();



        if (storableValues.Contains(v)) return true;

        if (storableValues.Contains(t)) return true;

        var i = (int)(object)e;
        if (storableValues.Contains(i.ToString())) return true;


        return false;
    }
}