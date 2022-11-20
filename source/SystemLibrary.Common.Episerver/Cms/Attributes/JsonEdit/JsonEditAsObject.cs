using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public static class StringExtensions
{
    public static T JsonEditAsObject<T>(this string text) where T : class
    {
        return text.Json<T>(new XhtmlStringJsonConverter());
    }
}

internal class XhtmlStringJsonConverter : JsonConverter<XhtmlString>
{
    public override XhtmlString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new XhtmlString(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, XhtmlString value, JsonSerializerOptions options)
    {
        if (value == null) return;

        writer.WriteStringValue(value.ToString());
    }
}