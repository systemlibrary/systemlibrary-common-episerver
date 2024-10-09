using System.Text;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Abstract;
using SystemLibrary.Common.Episerver.FontAwesome;

using static PackageConfig;
using static SystemLibrary.Common.Episerver.Attributes.ContentIconAttribute;

namespace SystemLibrary.Common.Episerver;

public partial class EditController : InternalBaseController
{
    const string CurrentFolder = "Cms/Edit";

    static FileContentResult StyleCache;

    public ActionResult Style()
    {
        AddCacheHeaders();

        if (IsCached(StyleCache)) return StyleCache;

        var edit = AppSettings.Current.Edit;

        var css = new StringBuilder("");

        css.Append(GetEmbeddedResource(CurrentFolder, "default.css"));
        css.Append(GetEmbeddedResource(CurrentFolder, "calendar-datetime-property-style.css"));

        AppendNewContentDialogItemColors(edit, css);

        AppendAllPropertiesShowPropertyDescriptions(edit, css);
        AppendAllPropertiesShowPropertiesAsColumns(edit, css);
        AppendAllPropertiesShowCheckBoxOnRightSide(edit, css);

        AppendVersionGadgetHideLanguageColumn(edit, css);

        AppendPageTreeIcons(edit, css);
        AppendPageTreeHideSitesTab(edit, css);

        AppendProjectBarActiveProjectBackgroundColor(edit, css);

        AppendActiveProjectBarBackgroundColor(edit, css);

        AppendAllPropertiesShowPropertiesAsColumns(edit, css);

        css.Append(Environment.NewLine + Environment.NewLine + FontAwesomeLoader.FontAwesomeBundledMinCss);

        return (StyleCache = GetFileContentResult(css, "text/css"));
    }

    void AppendProjectBarActiveProjectBackgroundColor(EditConfig edit, StringBuilder css)
    {
        if (edit.ProjectBarActiveProjectBackgroundColor.Is())
        {
            css.Append(GetEmbeddedResource(CurrentFolder, "ProjectBarActiveProjectBackgroundColor.css"));

            css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor) + "Border", edit.ProjectBarActiveProjectBackgroundColor.HexDarkenOrLighten(auto: true));
            css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor), edit.ProjectBarActiveProjectBackgroundColor);
        }
    }

    void AppendNewContentDialogItemColors(EditConfig edit, StringBuilder css)
    {
        if (edit.NewContentDialogItemBackgroundColor.Is() ||
            edit.NewContentDialogItemBorderColor.Is())
        {
            css.Append(GetEmbeddedResource(CurrentFolder, "NewContentDialogItemColors.css"));

            css.Replace(nameof(edit.NewContentDialogItemBorderColor), edit.NewContentDialogItemBorderColor);
            css.Replace(nameof(edit.NewContentDialogItemBackgroundColor), edit.NewContentDialogItemBackgroundColor);
        }
    }

    void AppendVersionGadgetHideLanguageColumn(EditConfig edit, StringBuilder css)
    {
        if (edit.VersionGadgetHideLanguageColumn)
            css.Append(GetEmbeddedResource(CurrentFolder, "VersionGadgetHideLanguageColumn.css"));
    }

    void AppendAllPropertiesShowCheckBoxOnRightSide(EditConfig edit, StringBuilder css)
    {
        if (edit.AllPropertiesShowCheckBoxOnRightSide)
        {
            css.Append(GetEmbeddedResource(CurrentFolder, "AllPropertiesShowCheckBoxOnRightSide.css"));

            if (edit.AllPropertiesShowPropertiesAsColumns)
                css.Append(GetEmbeddedResource(CurrentFolder, "AllPropertiesShowCheckBoxOnRightSideWhenPropertiesAreColumns.css"));
        }
    }

    void AppendAllPropertiesShowPropertyDescriptions(EditConfig edit, StringBuilder css)
    {
        if (edit.AllPropertiesShowPropertyDescriptions)
        {
            var note = "[EditController] BREAKING CHANGE: Optimizely CMS has broken 'allPropertiesShowPropertyDescriptions' in version 12.10, fixed in 12.19 and yet again broken in 12.20 and onwards - they've removed the hidden element that contained the description, I've opened up tickets, but they just go like 'sorry, cannot prioritize this now'. Set this flag to false and restart app";
            Log.Error(note);
            return;
        }

        if (edit.AllPropertiesShowPropertyDescriptions)
            css.Append(GetEmbeddedResource(CurrentFolder, "AllPropertiesShowPropertyDescriptions.css"));
    }

    void AppendActiveProjectBarBackgroundColor(EditConfig edit, StringBuilder css)
    {
        if (edit.ProjectBarActiveProjectBackgroundColor.IsNot()) return;

        css.Append(GetEmbeddedResource(CurrentFolder, "ProjectBarActiveProjectBackgroundColor.css"));

        css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor) + "Border", edit.ProjectBarActiveProjectBackgroundColor.HexDarkenOrLighten(auto: true));
        css.Replace(nameof(edit.ProjectBarActiveProjectBackgroundColor), edit.ProjectBarActiveProjectBackgroundColor);
    }

    void AppendAllPropertiesShowPropertiesAsColumns(EditConfig edit, StringBuilder css)
    {
        if (edit.AllPropertiesShowPropertiesAsColumns)
            css.Append(GetEmbeddedResource(CurrentFolder, "AllPropertiesShowPropertiesAsColumns.css"));
    }

    void AppendPageTreeHideSitesTab(EditConfig edit, StringBuilder css)
    {
        if (edit.PageTreeHideSitesTab)
            css.Append(GetEmbeddedResource(CurrentFolder, "PageTreeHideSitesTab.css"));
    }

    void AppendPageTreeIcons(EditConfig edit, StringBuilder css)
    {
        css.Append(GetEmbeddedResource(CurrentFolder, "PageTreeIcons.css"));

        css.Replace(nameof(edit.PageTreeSelectedContentBorderColor), edit.PageTreeSelectedContentBorderColor);

        if (ContentIconEditorDescriptorModule.ContentDescriptorSettings == null)
        {
            Log.Information("ContentDescriptorSettings is null, backround image in PageTree wont show. Try a restart of APP");
            return;
        }

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
