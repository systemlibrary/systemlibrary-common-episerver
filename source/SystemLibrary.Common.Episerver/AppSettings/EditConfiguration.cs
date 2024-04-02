namespace SystemLibrary.Common.Episerver;

public class EditConfiguration
{
    public bool NewContentDialogHideRequiredTitle { get; set; } = false;
    public bool AllPropertiesScrollableDocumentHeader { get; set; } = false;
    public bool AllPropertiesShowPropertyDescriptions { get; set; } = false;
    public bool AllPropertiesShowPropertiesAsColumns { get; set; } = false;
    public bool AllPropertiesShowCheckBoxOnRightSide { get; set; } = false;

    public bool VersionGadgetHideLanguageColumn { get; set; } = false;

    public string NewContentDialogItemBackgroundColor { get; set; } = "";
    public string NewContentDialogItemBorderColor { get; set; } = "";
    public string PageTreeSelectedContentBorderColor { get; set; } = "";
    public string ProjectBarActiveProjectBackgroundColor { get; set; } = "";

    public EditConfiguration()
    {
    }
}