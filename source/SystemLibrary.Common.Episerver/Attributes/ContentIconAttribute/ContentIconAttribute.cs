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
        //static string BackgroundColor = "#FAFAFA";
        static string ForegroundColor = "#000000";
        static string FolderForegroundColor = "#FFA000";
        static string SettingsForegroundColor = "#C7C7C7";

        static Enum[] FolderIcons => new Enum[] {
            FontAwesomeRegular.folder,
            FontAwesomeRegular.folder_open,
            FontAwesomeRegular.address_book,
            FontAwesomeSolid.folder,
            FontAwesomeSolid.folder_minus,
            FontAwesomeSolid.folder_open,
            FontAwesomeSolid.folder_plus
        };

        static Enum[] SettingIcons => new Enum[]
        {
            FontAwesomeSolid.screwdriver,
            FontAwesomeSolid.screwdriver_wrench,
            FontAwesomeRegular.hand_scissors
        };

        public Enum Value { get; set; }

        public ContentIconAttribute(string imageRelativeUrl) : base(imageRelativeUrl)
        {
        }

        public ContentIconAttribute(FontAwesomeRegular regular) : base(GetEmbeddedSvgName("~/SystemLibrary/Common/Episerver/ContentIcons/RegularIcon/", regular))
        {
            Value = regular;
        }

        public ContentIconAttribute(FontAwesomeSolid solid) : base(GetEmbeddedSvgName("~/SystemLibrary/Common/Episerver/ContentIcons/SolidIcon/", solid))
        {
            Value = solid;
        }

        public ContentIconAttribute(FontAwesomeBrands brand) : base(GetEmbeddedSvgName("~/SystemLibrary/Common/Episerver/ContentIcons/BrandsIcon/", brand))
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

        static string GetForegroundColor(Enum icon)
        {
            if (FolderIcons.Contains(icon))
                return FolderForegroundColor;


            if (SettingIcons.Contains(icon))
                return SettingsForegroundColor;

            return ForegroundColor;
        }
    }
}
