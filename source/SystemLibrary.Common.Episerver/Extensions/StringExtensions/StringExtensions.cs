using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Returns a string array with data stored from a JsonEdit attribute back to a list of class in C#
    /// </summary>
    /// <example>
    /// Convert the stored value from a JsonEdit attribute, which is stored as a string, to a List of T
    /// <code>
    /// class Car {
    ///   public string Name { get; set; }
    /// }
    /// [JsonEdit(Type = typeof(Car))]
    /// public virtual string Cars { get; set; }
    /// 
    /// public class Controller 
    /// {
    ///     ActionResult Index(Block currentBlock) //or page 
    ///     {
    ///         var cars = currentBlock.Cars.JsonEditAsObject&lt;List&lt;Car&gt;&gt;();
    ///         // cars is now a list of cars or empty
    ///     }
    /// }
    /// </code>
    /// </example>
    public static T JsonEditAsObject<T>(this string text) where T : class
    {
        return text.Json<T>(new XhtmlStringJsonConverter());
    }

    public static string ToFriendlyUrl(this string url)
    {
        if (url.IsNot()) return "";

        return "";
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