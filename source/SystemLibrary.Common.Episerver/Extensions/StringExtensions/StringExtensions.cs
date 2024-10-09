using System.Text.Json;
using System.Text.Json.Serialization;

using EPiServer.Core;
using EPiServer.Web.Routing;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// String extensions
/// </summary>
public static class StringExtensions
{
    static IUrlResolver _IUrlResolver;
    static IUrlResolver IUrlResolver => _IUrlResolver ??= Services.Get<IUrlResolver>();

    /// <summary>
    /// Convert the 'text', which is a string (json string) that comes from a 'public virtual' property marked with JsonEdit attribute
    /// <para>Read that json data as a List of T, or a simple T</para>
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
    /// <returns>Returns a simple object T or a List of T, based on the json string that comes from a 'public virtual' property marked with JsonEdit attribute</returns>
    public static T JsonEditAsObject<T>(this string text) where T : class
    {
        try
        {
            return text.Json<T>(new XhtmlStringJsonConverter());
        }
        catch
        {
            try
            {
                Log.Warning("[StringExtensions] JsonEditAsObject() warning thrown, with XhtmlStringJsonConverter");
                return text.Json<T>();
            }
            catch (Exception ex)
            {
                Log.Error("[StringExtensions] Could not convert '" + text + "' to " + typeof(T).Name + " in JsonEditAsObject(): " + ex.Message);
                return Activator.CreateInstance<T>();
            }
        }
    }

    /// <summary>
    /// Returns a friendly url version of the passed in url, usually a episerver content link or similar
    /// </summary>
    /// <param name="url"></param>
    /// <param name="convertToAbsolute">Pass null, which is default, to not convert, if input is absolute it returns absolute, else relative. Set to 'false' to force a relative return value, set to 'true' to force a absolute path based on CMS PrimaryDomain value</param>
    /// <returns>Returns friendly url, never null, minimum ""</returns>
    public static string ToFriendlyUrl(this string url, bool? convertToAbsolute = null)
    {
        if (url.IsNot()) return "";

        bool IsNativeHrefType()
        {
            return url.StartsWith("javascript:") || url.StartsWith("tel:") || url.StartsWith("email:");
        }

        if (IsNativeHrefType()) return url;

        bool IsEpiserverInternalLink()
        {
            return (url.Contains(",,") && url.Contains("epieditmode")) ||   // www.epi.com/image.jpg,,12345?epieditmode=true
                (url.Contains("/link/") && url.Contains(".aspx")) ||        // www.epi.com/Episerver/Cms/~/link/control.aspx
                                                                            // NOTE: These formats are already "public available paths", not internal links, co commented out
                                                                            // (url.Contains("globalassets/") && url.Contains(".")) ||    // www.epi.com/globalassets/image.jpg
                                                                            // (url.Contains("contentassets/") && url.Contains(".")) ||   // www.epi.com/contentassets/image.jpg
                (url.Contains("/cms/") && url.Contains("/contentversion")) || // www.epi.com/EPiServer/cms/Stores/contentversion/?contentLink=5
                (url.Contains("~/") && url.Contains("EPiServer/"));        //www.epi.com/Episerver/Cms/~/blockcontroller
        }

        void ConvertEpiserverInternalLink()
        {
            if (IsEpiserverInternalLink())
                url = IUrlResolver.GetUrl(url);
        }

        bool IsAbsolute()
        {
            return url.StartsWithAny("http:", "https:", "HTTP:", "HTTPS:", "Http:", "Https:");
        }

        string ConvertToAbsolute()
        {
            ConvertEpiserverInternalLink();

            if (IsAbsolute()) return url;

            if (url.StartsWith("/"))
                return BaseCms.PrimaryHostUrl + url;

            return BaseCms.PrimaryHostUrl + "/" + url;
        }

        string ConvertToRelative()
        {
            ConvertEpiserverInternalLink();

            if (!IsAbsolute()) return url;

            return new Uri(url).PathAndQuery;
        }

        string AsIs()
        {
            if (IsAbsolute())
                return ConvertToAbsolute();

            return ConvertToRelative();
        }

        if (convertToAbsolute == null)
            return AsIs();

        if (convertToAbsolute == false)
            return ConvertToRelative();

        return ConvertToAbsolute();
    }

    // TODO Test + consider blocks from a url?
    //public static T ToContent<T>(this string url) where T : ContentData
    //{
    //    if (url.IsNot()) return default;

    //    if (IUrlResolver == null) return default;

    //    var urlBuilder = new UrlBuilder(url);

    //    var content = IUrlResolver.Route(urlBuilder);

    //    return content as T;
    //}
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