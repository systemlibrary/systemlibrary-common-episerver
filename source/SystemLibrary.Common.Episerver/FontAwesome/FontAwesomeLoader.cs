using System;
using System.Reflection;

namespace SystemLibrary.Common.Episerver.FontAwesome;

internal class FontAwesomeLoader
{
    internal static string FontAwesomeBundledMinCss;
    internal static byte[] Brands400Woff2;
    internal static byte[] Regular400Woff2;
    internal static byte[] Solid900Woff2;

    static FontAwesomeLoader()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();

        var cssFolder = "FontAwesome/Css";

        FontAwesomeBundledMinCss = Net.Assemblies.GetEmbeddedResource(cssFolder, "fontawesome-bundled.css", currentAssembly);

        var fontFolder = "FontAwesome/Fonts";

        Brands400Woff2 = Net.Assemblies.GetEmbeddedResourceAsBytes(fontFolder, "fa-brands-400.woff2", currentAssembly);
        Regular400Woff2 = Net.Assemblies.GetEmbeddedResourceAsBytes(fontFolder, "fa-regular-400.woff2", currentAssembly);
        Solid900Woff2 = Net.Assemblies.GetEmbeddedResourceAsBytes(fontFolder, "fa-solid-900.woff2", currentAssembly);
    }

    internal static string GetFontAwesomeIconRequestUrl(Enum icon)
    {
        string requestUrl;

        if (icon is FontAwesomeBrands)
        {
            requestUrl = "~/SystemLibrary/CommonEpiserverFontAwesome/FontAwesome/BrandsIcon/";
        }
        else if (icon is FontAwesomeRegular)
        {
            requestUrl = "~/SystemLibrary/CommonEpiserverFontAwesome/FontAwesome/RegularIcon/";
        }
        else
        {
            requestUrl = "~/SystemLibrary/CommonEpiserverFontAwesome/FontAwesome/SolidIcon/";
        }

        var iconName = icon.ToString();

        if (iconName.EndsWithAny(".svg", ".jpg", ".png"))
        {
            if (iconName.StartsWith("~"))
                iconName = iconName.Substring(1);

            iconName = iconName.Replace("/", "__enum_");

            return requestUrl + iconName;
        }

        //SVG's might have a digit as first character
        //Enum cannot start with a digit, hence swapping it with '__enum_'
        //Note: could also have removed the 10 svg's that has a number as start... avoiding stupid logic
        if (iconName.StartsWith("__enum_"))
            iconName = iconName.Substring(3);

        return requestUrl + iconName.Replace("_", "-") + ".svg";
    }
}
