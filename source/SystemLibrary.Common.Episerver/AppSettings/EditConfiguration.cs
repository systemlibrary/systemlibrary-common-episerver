namespace SystemLibrary.Common.Episerver;

public class EditConfiguration
{
    public bool NewContentDialogHideRequiredTitle { get; set; } = true;
    public bool AllPropertiesShowPropertyDescriptions { get; set; } = true;
    public bool AllPropertiesShowPropertiesAsColumns { get; set; } = true;
    public bool AllPropertiesShowCheckBoxOnRightSide { get; set; } = true;

    public bool VersionGadgetHideLanguageColumn { get; set; } = true;

    public string NewContentDialogItemBackgroundColor { get; set; } = "";
    public string NewContentDialogItemBorderColor { get; set; } = "";
    public string PageTreeSelectedContentBorderColor { get; set; } = "";
    public string ProjectBarActiveProjectBackgroundColor { get; set; } = "";

    public EditConfiguration()
    {
    }
}