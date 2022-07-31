using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

internal static class JsonEditPropertiesLoader
{
    internal static string GetProperties(Type type)
    {
        if (!type.IsClass)
            throw new Exception("Type " + type.Name + " is not a class, cannot continue loading the jsonedit editor");

        var properties = type.GetProperties();

        var definitions = new StringBuilder();

        if (properties?.Length < 1) return definitions.ToString();

        definitions.Append("\"properties\": {" + Environment.NewLine);

        foreach (var property in properties)
        {
            var added = AddProperty(property);
            if (added.Is())
            {
                definitions.Append(Environment.NewLine);
                definitions.Append(added);
            }
        }

        definitions.TrimEnd(",");

        definitions.Append("},");

        definitions.Append(Environment.NewLine + "\"required\": [");

        foreach (var property in properties)
        {
            var required = AddRequiredPropertyName(property);
            if (required.Is())
            {
                definitions.Append(required + ",");
            }
        }

        definitions.TrimEnd(",");
        definitions.Append("]");

        return definitions.ToString();
    }

    static string AddProperty(PropertyInfo property)
    {
        var type = property.PropertyType;
        if (!IsValidDefinitionType(type))
        {
            Dump.Write("Unsupported type: " + type.Name + ", for property: " + property.Name);
            return null;
        }

        var jsonProperty = GetDefinitionName(property, type);

        if (type.IsEnum)
            jsonProperty += GetDefinitionEnumSelections(type);
        else
            jsonProperty += GetDefinitionUiHints(property, type);

        jsonProperty += Environment.NewLine + "},";
        return jsonProperty;
    }


    //"name": { "type": "string",
    static string GetDefinitionName(PropertyInfo property, Type type)
    {
        return "\"" + property.Name + "\": { " + Environment.NewLine + "\"type\": \"" + GetTypeName(type) + "\"," + Environment.NewLine;
    }

    static string GetDefinitionEnumSelections(Type type)
    {
        var sb = new StringBuilder(string.Join(",", type.GetEnumNames().Select(x => "\"" + x + "\"")));

        sb.TrimEnd(",");

        return "\"enum\": [" + sb + "]";
    }

    static string GetTypeName(Type type)
    {
        if (type == SystemType.IntType) return "integer";
        if (type == typeof(double)) return "number";

        if (type == SystemType.DateTimeOffsetType || type == SystemType.DateTimeType) return "string";

        if (type.IsEnum) return "string";

        return type.Name.ToLower();
    }

    //"ui": {
    //  "displayName": "Some name",
    //  "placeholderHint": "Some description hint",
    //}
    static string GetDefinitionUiHints(PropertyInfo property, Type type)
    {
        var attribute = property.GetCustomAttribute<DisplayAttribute>();
        var ui = "\"ui\": {";
        if (attribute != null)
        {
            if (attribute.Name.Is())
                ui += Environment.NewLine + "\"displayName\": \"" + attribute.Name + "\",";

            if (attribute.Description.Is())
                ui += Environment.NewLine + "\"placeholderHint\": \"" + attribute.Description + "\",";
        }

        var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
        if (requiredAttribute != null && requiredAttribute.ErrorMessage.Is())
        {
            ui += Environment.NewLine + "\"validationHint\": \"" + requiredAttribute.ErrorMessage + "\",";
        }

        if (type == SystemType.DateTimeOffsetType || type == SystemType.DateTimeType)
        {
            ui += Environment.NewLine + "\"editor\": \"date\",";
        }

        if (ui.EndsWith(','))
            ui = ui.Substring(0, ui.Length - 1);

        return ui + "}";
    }

    static bool IsValidDefinitionType(Type type)
    {
        return type == SystemType.StringType ||
            type == SystemType.IntType ||
            type == SystemType.BoolType ||
            type == SystemType.DateTimeOffsetType ||
            type == SystemType.DateTimeType ||
            type == typeof(double) ||
            type.IsEnum;
    }

    static string AddRequiredPropertyName(PropertyInfo property)
    {
        var hasRequiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
        if (hasRequiredAttribute != null && !hasRequiredAttribute.AllowEmptyStrings)
        {
            return "\"" + property.Name + "\"";
        }
        return null;
    }
}