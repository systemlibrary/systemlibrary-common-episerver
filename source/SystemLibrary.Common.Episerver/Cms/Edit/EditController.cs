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

        var css = new StringBuilder("");
        if (edit.ShowEditStyle)
        {
            css.Append(GetEmbeddedResource(CurrentFolder, "showEditStyle.css"));

            var hideLanguageColumnInVersionGadget = edit.HideLanguageColumnInVersionGadget ? "0px" : "";
            var hideLanguageColumnInVersionGadgetVisibility = edit.HideLanguageColumnInVersionGadget ? "hidden" : "visible";

            css.Replace(nameof(edit.HideLanguageColumnInVersionGadget) + "Visibility", hideLanguageColumnInVersionGadgetVisibility);
            css.Replace(nameof(edit.HideLanguageColumnInVersionGadget), hideLanguageColumnInVersionGadget);
            css.Replace(nameof(edit.ContentTitleColor), edit.ContentTitleColor);
            css.Replace(nameof(edit.ContentCreationBorderColor), edit.ContentCreationBorderColor);
            css.Replace(nameof(edit.ContentCreationBackgroundColor), edit.ContentCreationBackgroundColor);
            css.Replace(nameof(edit.PageTreeSelectedContentBorderColor), edit.PageTreeSelectedContentBorderColor);
        }

        AppendActiveProjectBarBackgroundColor(edit, css);

        AppendShowEditFieldsAsColumns(edit, css);

        AppendCalendarDateTimePropertyStyle(css);

        AppendCustomPageTreeIcons(css);

        css.Append(System.Environment.NewLine + System.Environment.NewLine + FontAwesomeLoader.FontAwesomeBundledMinCss);

        return (StyleCache = GetFileContentResult(css, "text/css"));
    }

    void AppendActiveProjectBarBackgroundColor(EditConfiguration edit, StringBuilder sb)
    {
        if (edit.ActiveProjectBarBackgroundColor.IsNot()) return;

        sb.Append(GetEmbeddedResource(CurrentFolder, "showActiveProjectBarBackgroundColor.css"));

        sb.Replace(nameof(edit.ActiveProjectBarBackgroundColor) + "Border", edit.ActiveProjectBarBackgroundColor.HexDarkenOrLighten(auto: true));
        sb.Replace(nameof(edit.ActiveProjectBarBackgroundColor), edit.ActiveProjectBarBackgroundColor);
    }

    void AppendShowEditFieldsAsColumns(EditConfiguration edit, StringBuilder sb)
    {
        if (!edit.ShowEditFieldsAsColumns) return;

        sb.Append(GetEmbeddedResource(CurrentFolder, "showEditFieldsAsColumns.css"));
    }

    void AppendCalendarDateTimePropertyStyle(StringBuilder sb)
    {
        sb.Append(GetEmbeddedResource(CurrentFolder, "calendar-datetime-property-style.css"));
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
