using System.Text;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;
using SystemLibrary.Common.Episerver.FontAwesome;

using static SystemLibrary.Common.Episerver.Cms.Attributes.ContentIconAttribute;

namespace SystemLibrary.Common.Episerver.Cms;

public partial class EditController : BaseController
{
    const string CurrentFolder = "Cms/Edit";

    static FileContentResult StyleCache;

    public ActionResult Style()
    {
        AddCacheHeaders();

        if (IsCached(StyleCache)) return StyleCache;

        var edit = AppSettings.Current.Edit;

        var css = GetEmbeddedResource(CurrentFolder, "Style.css");

        var hideLanguageColumnInVersionGadget = edit.HideLanguageColumnInVersionGadget ? "0px" : "";
        var hideLanguageColumnInVersionGadgetVisibility = edit.HideLanguageColumnInVersionGadget ? "hidden" : "visible";

        css.Replace(nameof(edit.HideLanguageColumnInVersionGadget) + "Visibility", hideLanguageColumnInVersionGadgetVisibility);
        css.Replace(nameof(edit.HideLanguageColumnInVersionGadget), hideLanguageColumnInVersionGadget);
        css.Replace(nameof(edit.ContentTitleColor), edit.ContentTitleColor);
        css.Replace(nameof(edit.ContentCreationBorderColor), edit.ContentCreationBorderColor);
        css.Replace(nameof(edit.ContentCreationBackgroundColor), edit.ContentCreationBackgroundColor);
        css.Replace(nameof(edit.PageTreeSelectedContentBorderColor), edit.PageTreeSelectedContentBorderColor);

        if (edit.ActiveProjectBarBackgroundColor.Is())
        {
            css.Replace(nameof(edit.ActiveProjectBarBackgroundColor) + "Border", edit.ActiveProjectBarBackgroundColor.HexDarkenOrLighten(auto: true));
            css.Replace(nameof(edit.ActiveProjectBarBackgroundColor), edit.ActiveProjectBarBackgroundColor);
        }

        AppendShowEditFieldsAsColumns(edit, css);

        AppendCustomPageTreeIcons(css);

        css.Append(System.Environment.NewLine + System.Environment.NewLine + FontAwesomeLoader.FontAwesomeBundledMinCss);

        return (StyleCache = GetFileContentResult(css, "text/css"));
    }

    void AppendShowEditFieldsAsColumns(EditConfiguration edit, StringBuilder sb)
    {
        if (!edit.ShowEditFieldsAsColumns) return;

        sb.Append(GetEmbeddedResource(CurrentFolder, "showEditFieldsAsColumns.css"));
    }

    static void AppendCustomPageTreeIcons(StringBuilder sb)
    {
        if (ContentIconEditorDescriptorModule.ContentDescriptorSettings == null) return;

        foreach (var setting in ContentIconEditorDescriptorModule.ContentDescriptorSettings)
        {
            if (setting.Item4.Is())
            {
                sb.Append("." + setting.Item3.Replace(" ", "."));

                var url = setting.Item4;

                if (url.StartsWith("~"))
                    url = url.Substring(1);

                if (!url.StartsWith("/"))
                    url = "/" + url;

                sb.Append("{background-size: 100%;background-image: url(" + url + ");}");
            }
        }
    }
}
