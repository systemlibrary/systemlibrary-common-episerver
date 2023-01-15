namespace SystemLibrary.Common.Episerver;

public class EditConfiguration
{
    public bool ShowEditFieldsAsColumns { get; set; } = false;
    public bool HideLanguageColumnInVersionGadget { get; set; } = false;
    public string ContentCreationBackgroundColor { get; set; } = "";
    public string ContentCreationBorderColor { get; set; } = "";
    public string PageTreeSelectedContentBorderColor { get; set; } = "";
    public string ContentTitleColor { get; set; } = "";
    public string ActiveProjectBarBackgroundColor { get; set; } = "";

    public EditConfiguration()
    {
    }
}