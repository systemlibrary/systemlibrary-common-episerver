using SystemLibrary.Common.Episerver.Properties;

internal class PackageConfig
{
    public bool Debug { get; set; }
    public EditConfig Edit { get; set; }
    public SsrConfig Ssr { get; set; }
    public PropertiesConfig Properties { get; set; }

    public PackageConfig()
    {
        Edit = new EditConfig();
        Ssr = new SsrConfig();
        Properties = new PropertiesConfig();
    }

    internal class EditConfig
    {
        public bool NewContentDialogHideRequiredTitle { get; set; } = false;
        public bool AllPropertiesShowPropertyDescriptions { get; set; } = false;
        public bool AllPropertiesShowPropertiesAsColumns { get; set; } = false;
        public bool AllPropertiesShowCheckBoxOnRightSide { get; set; } = false;

        public bool VersionGadgetHideLanguageColumn { get; set; } = false;

        public string NewContentDialogItemBackgroundColor { get; set; } = "";
        public string NewContentDialogItemBorderColor { get; set; } = "";
        public string PageTreeSelectedContentBorderColor { get; set; } = "";
        public bool PageTreeHideSitesTab { get; set; } = false;
        public string ProjectBarActiveProjectBackgroundColor { get; set; } = "";
    }

    internal class SsrConfig
    {
        public bool ShowComponentEditLink { get; set; } = false;
    }

    internal class PropertiesConfig
    {
        public MessageConfig Message { get; set; }

        public PropertiesConfig()
        {
            Message = new MessageConfig();
        }
    }

}
