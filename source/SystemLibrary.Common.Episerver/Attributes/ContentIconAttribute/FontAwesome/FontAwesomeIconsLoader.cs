using System.Reflection;

namespace SystemLibrary.Common.Episerver.Attributes
{
    partial class ContentIconAttribute
    {
        internal class FontAwesomeIconsLoader
        {
            internal static string FontAwesomeBundledMinCss;
            internal static byte[] Brands400Woff2;
            internal static byte[] Regular400Woff2;
            internal static byte[] Solid900Woff2;

            static FontAwesomeIconsLoader()
            {
                var currentAssembly = Assembly.GetExecutingAssembly();

                var cssFolder = "Attributes/ContentIconAttribute/FontAwesome/Css";
                FontAwesomeBundledMinCss = Net.Assemblies.GetEmbeddedResource(cssFolder, "fontawesome-bundled.min.css", currentAssembly);

                var webFontsFolder = "Attributes/ContentIconAttribute/FontAwesome/WebFonts";
                Brands400Woff2 = Net.Assemblies.GetEmbeddedResourceAsBytes(webFontsFolder, "fa-brands-400.woff2", currentAssembly);
                Regular400Woff2 = Net.Assemblies.GetEmbeddedResourceAsBytes(webFontsFolder, "fa-regular-400.woff2", currentAssembly);
                Solid900Woff2 = Net.Assemblies.GetEmbeddedResourceAsBytes(webFontsFolder, "fa-solid-900.woff2", currentAssembly);
            }
        }
    }
}
