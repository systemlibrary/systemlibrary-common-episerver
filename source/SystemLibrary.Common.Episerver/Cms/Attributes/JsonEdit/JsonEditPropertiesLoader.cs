using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

using EPiServer.Core;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

internal static class JsonEditPropertiesLoader
{
    internal static string GetPropertySchema(Type type)
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
        if (!IsValidVariableType(type))
        {
            if (IsSimpleClassObject(type))
            {
                var child = new StringBuilder("\"" + property.Name + "\": {");
                child.Append("\"type\": \"object\",");
                child.Append(GetPropertySchema(type));

                child.Append("},");
                return child.ToString();
            }
            else
            {
                Log.Error("Unsupported type: " + type.Name + ", for property: " + property.Name + ". The Json Editor supports only simple types: string, int, double, datetime, datetimeoffset, bool, Enum or a Class variable which only have any of those variable types");
                return null;
            }
        }

        var jsonProperty = new StringBuilder(GetDefinitionName(property, type));

        if (type.IsEnum)
            jsonProperty.Append(GetDefinitionEnumSelections(type));
        else
            jsonProperty.Append(GetDefinitionUiHints(property, type));

        jsonProperty.Append(Environment.NewLine + "},");

        return jsonProperty.ToString();
    }

    //"name": { "type": "string", ... }
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

        if (type == typeof(XhtmlString)) return "textarea";

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

    static bool IsValidVariableType(Type type)
    {
        return
            type == typeof(XhtmlString) ||
            type == SystemType.StringType ||
            type == SystemType.IntType ||
            type == SystemType.BoolType ||
            type == SystemType.DateTimeOffsetType ||
            type == SystemType.DateTimeType ||
            type == typeof(double) ||
            type.IsEnum;
    }

    static bool IsSimpleClassObject(Type type)
    {
        if (!type.IsClass ||
            type.IsValueType ||
            type.IsPrimitive ||
            type.IsGenericType ||
            type.IsGenericParameter ||
            type.IsGenericTypeDefinition ||
            type.IsByRef ||
            type.IsMarshalByRef ||
            type.IsListOrArray() ||
            type.IsDictionary() ||
            type.IsPointer ||
            type.IsAbstract ||
            !type.IsTypeDefinition ||
            type.IsVariableBoundArray ||
            type.IsInterface)
            return false;

        return true;
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