using System.Text;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;

using static SystemLibrary.Common.Episerver.Cms.Attributes.ContentIconAttribute;

namespace SystemLibrary.Common.Episerver.Cms;

public partial class EditController : BaseController
{
    const string CmsEditFolder = "Cms/Edit";

    static FileContentResult CssCache;

    public ActionResult Style()
    {
        //TODO: Merge this together with FontAwesomeController/Style ?
        // then one have only 1 line in the module.config
        AddCacheHeaders();

        if (CssCache != null) return CssCache;

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        var css = new StringBuilder("");
        if (cmsEdit.Enabled)
        {
            css = GetEmbeddedResource(CmsEditFolder, "Style.css");

            var hideLanguageColumnInVersionGadget = cmsEdit.HideLanguageColumnInVersionGadget ? "0px" : "";
            var hideLanguageColumnInVersionGadgetVisibility = cmsEdit.HideLanguageColumnInVersionGadget ? "hidden" : "visible";

            css.Replace(nameof(cmsEdit.HideLanguageColumnInVersionGadget) + "Visibility", hideLanguageColumnInVersionGadgetVisibility);
            css.Replace(nameof(cmsEdit.HideLanguageColumnInVersionGadget), hideLanguageColumnInVersionGadget);
            css.Replace(nameof(cmsEdit.ContentTitleColor), cmsEdit.ContentTitleColor);
            css.Replace(nameof(cmsEdit.ContentCreationBorderColor), cmsEdit.ContentCreationBorderColor);
            css.Replace(nameof(cmsEdit.ContentCreationBackgroundColor), cmsEdit.ContentCreationBackgroundColor);
            css.Replace(nameof(cmsEdit.PageTreeSelectedContentBorderColor), cmsEdit.PageTreeSelectedContentBorderColor);
            
            if (cmsEdit.ActiveProjectBarBackgroundColor.Is())
            {
                try
                {
                    css.Replace(nameof(cmsEdit.ActiveProjectBarBackgroundColor) + "Border", cmsEdit.ActiveProjectBarBackgroundColor.HexDarkenOrLighten(auto: true));
                }
                catch
                {
                }
                css.Replace(nameof(cmsEdit.ActiveProjectBarBackgroundColor), cmsEdit.ActiveProjectBarBackgroundColor);
            }

            AppendCustomPageTreeIcons(css);
        }

        return (CssCache = GetFileContentResult(css, "text/css"));
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
