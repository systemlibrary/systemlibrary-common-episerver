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

        AppendNewContentDialogHideRequiredTitle(edit, css);
        AppendAllPropertiesShowPropertyDescriptions(edit, css);
        AppendAllPropertiesShowPropertiesAsColumns(edit, css);
        AppendAllPropertiesShowCheckBoxOnRightSide(edit, css);
        AppendVersionGadgetHideLanguageColumn(edit, css);

        AppendNewContentDialogItemBackgroundColor(edit, css);

        AppendCalendarDateTimePropertyStyle(css);

        AppendCustomPageTreeIcons(edit, css);

        AppendProjectBarActiveProjectBackgroundColor(edit, css);

        AppendContentTitleColor(edit, css);

        AppendActiveProjectBarBackgroundColor(edit, css);

        AppendAllPropertiesShowPropertiesAsColumns(edit, css);



        css.Append(System.Environment.NewLine + System.Environment.NewLine + FontAwesomeLoader.FontAwesomeBundledMinCss);

        return (StyleCache = GetFileContentResult(css, "text/css"));
    }

    void AppendContentTitleColor(EditConfiguration edit, StringBuilder css)
    {
        if (edit.ContentTitleColor.Is())
        {
            css.Append(GetEmbeddedResource(CurrentFolder, "ContentTitleColor.css"));
            css.Replace(nameof(edit.ContentTitleColor), edit.ContentTitleColor);
        }
    }

    void AppendProjectBarActiveProjectBackgroundColor(EditConfiguration edit, StringBuilder css)
    {
        if(edit.ProjectBarActiveProjectBackgroundColor.Is())
        {
            css.Append(GetEmbeddedResource(CurrentFolder, "ProjectBarActiveProjectBackgroundColor.css"));

            css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor) + "Border", edit.ProjectBarActiveProjectBackgroundColor.HexDarkenOrLighten(auto: true));
            css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor), edit.ProjectBarActiveProjectBackgroundColor);
        }
    }

    void AppendNewContentDialogItemBackgroundColor(EditConfiguration edit, StringBuilder css)
    {
        if (edit.NewContentDialogItemBackgroundColor.Is() ||
            edit.NewContentDialogItemBorderColor.Is())
        {
            css.Append(GetEmbeddedResource(CurrentFolder, "NewContentDialogItemBackgroundColor.css"));

            css.Replace(nameof(edit.NewContentDialogItemBorderColor), edit.NewContentDialogItemBorderColor);
            css.Replace(nameof(edit.NewContentDialogItemBackgroundColor), edit.NewContentDialogItemBackgroundColor);
        }
    }

    void AppendVersionGadgetHideLanguageColumn(EditConfiguration edit, StringBuilder css)
    {
        if (edit.VersionGadgetHideLanguageColumn)
            css.Append(GetEmbeddedResource(CurrentFolder, "VersionGadgetHideLanguageColumn.css"));
    }

    void AppendAllPropertiesShowCheckBoxOnRightSide(EditConfiguration edit, StringBuilder css)
    {
        if (edit.AllPropertiesShowCheckBoxOnRightSide)
            css.Append(GetEmbeddedResource(CurrentFolder, "AllPropertiesShowCheckBoxOnRightSide.css"));
    }

    void AppendAllPropertiesShowPropertyDescriptions(EditConfiguration edit, StringBuilder css)
    {
        if(edit.AllPropertiesShowPropertyDescriptions)
            css.Append(GetEmbeddedResource(CurrentFolder, "AllPropertiesShowPropertyDescriptions.css"));
    }

    void AppendNewContentDialogHideRequiredTitle(EditConfiguration edit, StringBuilder css)
    {
        if(edit.NewContentDialogHideRequiredTitle)
            css.Append(GetEmbeddedResource(CurrentFolder, "NewContentDialogHideRequiredTitle.css"));
    }

    void AppendActiveProjectBarBackgroundColor(EditConfiguration edit, StringBuilder css)
    {
        if (edit.ProjectBarActiveProjectBackgroundColor.IsNot()) return;

        css.Append(GetEmbeddedResource(CurrentFolder, "ProjectBarActiveProjectBackgroundColor.css"));

        css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor) + "Border", edit.ProjectBarActiveProjectBackgroundColor.HexDarkenOrLighten(auto: true));
        css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor), edit.ProjectBarActiveProjectBackgroundColor);
    }

    void AppendAllPropertiesShowPropertiesAsColumns(EditConfiguration edit, StringBuilder css)
    {
        if (edit.AllPropertiesShowPropertiesAsColumns)
            css.Append(GetEmbeddedResource(CurrentFolder, "AllPropertiesShowPropertiesAsColumns.css"));
    }

    void AppendCalendarDateTimePropertyStyle(StringBuilder sb)
    {
        sb.Append(GetEmbeddedResource(CurrentFolder, "calendar-datetime-property-style.css"));
    }

    void AppendCustomPageTreeIcons(EditConfiguration edit, StringBuilder css)
    {
        css.Append(GetEmbeddedResource(CurrentFolder, "CustomPageTreeIcons.css"));

        css.Replace(nameof(edit.PageTreeSelectedContentBorderColor), edit.PageTreeSelectedContentBorderColor);

        if (ContentIconEditorDescriptorModule.ContentDescriptorSettings == null) return;

        foreach (var setting in ContentIconEditorDescriptorModule.ContentDescriptorSettings)
        {
            if (setting.Item4.Is())
            {
                css.Append("." + setting.Item3.Replace(" ", "."));

                var url = setting.Item4;

                if (url.StartsWith("~"))
                    url = url.Substring(1);

                if (!url.StartsWith("/"))
                    url = "/" + url;

                css.Append("{background-size: 100%;background-image: url(" + url + ");}");
            }
        }
    }
}
