using System;
using System.Linq;

using EPiServer.DataAnnotations;

namespace SystemLibrary.Common.Episerver.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public partial class ContentIconAttribute : ImageUrlAttribute
    {
        static string BackgroundColor = "#FAFAFA";
        static string ForegroundColor = "#000000";
        static string FolderForegroundColor = "#FFA000";
        static string SettingsForegroundColor = "#C7C7C7";

        static int FontSize = 55;

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

        public Enum Value { get; }

        public ContentIconAttribute(FontAwesomeRegular regular) : base(GetEmbeddedSvgName("~/SystemLibrary/ContentIcons/RegularIcon/", regular))
        {
            Value = regular;
        }

        public ContentIconAttribute(FontAwesomeSolid solid) : base(GetEmbeddedSvgName("~/SystemLibrary/ContentIcons/SolidIcon/", solid))
        {
            Value = solid;
        }

        static string GetEmbeddedSvgName(string requestUrl, Enum icon)
        {
            var iconName = icon.ToString();
            //Some svg's have a digit as first letter of the filename
            //enums cannot start with digits, hence those are manually prefixed with ___
            if(iconName.StartsWith("___"))
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
