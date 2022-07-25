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
}
