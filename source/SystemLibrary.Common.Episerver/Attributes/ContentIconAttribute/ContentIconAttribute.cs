using System;
using System.Linq;

using EPiServer.DataAnnotations;

namespace SystemLibrary.Common.Episerver.Attributes
{
    /// <summary>
    /// Chose one of Font Awesome v. 6's free icons for your Blocks and Pages
    /// - Font Awesome Icons will also display in Page Tree
    /// - You can pass a custom path to your own image, but that wont, per now, show up in Page Tree
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// [ContentIcon(ContentIconAttribute.FontAwesomeRegular.credit_card)]
    /// public class StartPage : PageData
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public partial class ContentIconAttribute : ImageUrlAttribute
    {
        //Note: These colors are manually written into the SVG files themselves
        //because the "New content dialog" loads the SVG's inside an img tag
        //while we also need to apply a separate css class for these icons, for the colors to be added in page tree
        static string FolderForegroundColor = "#FFA000";
        static string SettingsForegroundColor = "#C7C7C7";

        static Enum[] FolderIcons => new Enum[] {
            FontAwesomeRegular.folder,
            FontAwesomeRegular.folder_open,
            FontAwesomeSolid.folder,
            FontAwesomeSolid.folder_minus,
            FontAwesomeSolid.folder_open,
            FontAwesomeSolid.folder_plus,
            FontAwesomeSolid.folder_tree
        };

        static Enum[] SettingIcons => new Enum[]
        {
            FontAwesomeSolid.screwdriver,
            FontAwesomeSolid.screwdriver_wrench
        };

        public Enum Value { get; set; }

        internal string IconRelativeUrl;

        public ContentIconAttribute(string iconRelativeUrl) : base(iconRelativeUrl)
        {
            IconRelativeUrl = iconRelativeUrl;
        }

        public ContentIconAttribute(FontAwesomeRegular regular) : base(GetEmbeddedSvgName("~/SystemLibrary/Common/Episerver/ContentIconAttribute/RegularIcon/", regular))
        {
            Value = regular;
        }

        public ContentIconAttribute(FontAwesomeSolid solid) : base(GetEmbeddedSvgName("~/SystemLibrary/Common/Episerver/ContentIconAttribute/SolidIcon/", solid))
        {
            Value = solid;
        }

        public ContentIconAttribute(FontAwesomeBrands brand) : base(GetEmbeddedSvgName("~/SystemLibrary/Common/Episerver/ContentIconAttribute/BrandsIcon/", brand))
        {
            Value = brand;
        }

        static string GetEmbeddedSvgName(string requestUrl, Enum icon)
        {
            return GetEmbeddedSvgName(requestUrl, icon.ToString());
        }

        static string GetEmbeddedSvgName(string requestUrl, string iconName)
        {
            if (iconName.EndsWithAny(".svg", ".jpg", ".png"))
            {
                if (iconName.StartsWith("~"))
                    iconName = iconName.Substring(1);
                iconName = iconName.Replace("/", "___");

                return requestUrl + iconName;
            }

            //SVG's might have a digit as first character
            //Enum cannot start with a digit, hence swapping it with '___'
            if (iconName.StartsWith("___"))
                iconName = iconName.Substring(3);

            return requestUrl + iconName.Replace("_", "-") + ".svg";
        }
        
        internal bool IsFolder()
        {
            if (Value == null) return false;

            return FolderIcons.Contains(Value);
        }

        internal bool IsSettings()
        {
            if (Value == null) return false;

            return SettingIcons.Contains(Value);
        }
    }
}
