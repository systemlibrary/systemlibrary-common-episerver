using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string PropsToJsonProps(object props, bool camelCaseProps)
    {
        var options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            MaxDepth = 32,
            AllowTrailingCommas = true,
            WriteIndented = false,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = camelCaseProps ? JsonNamingPolicy.CamelCase : null,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        return props.Json(options);
    }
}