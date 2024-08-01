using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using EPiServer.Core;
using EPiServer.Web.Routing;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class StringExtensions
{
    static IUrlResolver _IUrlResolver;
    static IUrlResolver IUrlResolver =>
        _IUrlResolver != null ? _IUrlResolver :
        (_IUrlResolver = Services.Get<IUrlResolver>());

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
        try
        {
            return text.Json<T>(new XhtmlStringJsonConverter());
        }
        catch
        {
            try
            {
                Log.Warning("JsonEditAsObject() warning thrown, with XhtmlStringJsonConverter");
                return text.Json<T>();
            }
            catch(Exception ex)
            {
                Log.Error("Could not convert '" + text + "' to " + typeof(T).Name + " in JsonEditAsObject(): " + ex.Message);
                return Activator.CreateInstance<T>();
            }
        }
    }

    /// <summary>
    /// Returns true if path is either relative or absolute path to a file
    /// 
    /// File must be minimum 4 char long in total, including the extension and the dot
    /// 
    /// Supports up to 6 characters in the extension
    /// 
    /// Supports also query-params in the path
    /// 
    /// Returns true or false
    /// </summary>
    public static bool IsFile(this string path)
    {
        if (path.IsNot()) return false;

        var length = path.Length;
        if (length <= 3) return false;

        var hasInvalidPathChars = path.IndexOfAny(Path.GetInvalidPathChars()) != -1;
        if (hasInvalidPathChars) return false;

        var extensionIndex = path.LastIndexOf('.');

        if (extensionIndex == -1) return false;

        var queryIndex = path.IndexOf('?');

        if (queryIndex == -1)
        {
            if (extensionIndex == length - 1) return false;

            var lastIndexOfSlash = path.LastIndexOf('/');
            if (lastIndexOfSlash > extensionIndex) return false;

            return extensionIndex >= length - 7; // .config
        }

        if(extensionIndex > queryIndex)
        {
            var temp = path.Substring(0, queryIndex);
            
            return temp.LastIndexOf(".") >= temp.Length - 7; // .config
        }

        return queryIndex - 7 <= extensionIndex;
    }

    /// <summary>
    /// Returns a friendly url version of the passed in url, usually a episerver content link or similar
    /// 
    /// Returns never null even if input is null
    /// </summary>
    /// <param name="url"></param>
    /// <param name="convertToAbsolute">Pass null, which is default, to not convert, if input is absolute it returns absolute, else relative. Set to 'false' to force a relative return value, set to 'true' to force a absolute path based on CMS PrimaryDomain value</param>
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