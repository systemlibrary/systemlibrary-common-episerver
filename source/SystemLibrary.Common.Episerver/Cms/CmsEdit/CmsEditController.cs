using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using static SystemLibrary.Common.Episerver.Cms.Attributes.ContentIconAttribute;

namespace SystemLibrary.Common.Episerver;

public partial class CmsEditController : Controller
{
    const string CmsEditFolder = "Cms/CmsEdit";

    static int ClientCacheSeconds = 43200;

    static Assembly _CurrentAssembly;

    static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
        (_CurrentAssembly = Assembly.GetExecutingAssembly());

    static FileContentResult StylesheetCache;

    public ActionResult Stylesheet()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

        if (StylesheetCache != null) return StylesheetCache;

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        var sb = new StringBuilder("");
        if (cmsEdit.Enabled)
        {
            sb.Append(Net.Assemblies.GetEmbeddedResource(CmsEditFolder, "CmsEditStylesheet.css", CurrentAssembly));

            var hideLanguageColumnInVersionGadget = cmsEdit.HideLanguageColumnInVersionGadget ? "0px" : "";
            var hideLanguageColumnInVersionGadgetVisibility = cmsEdit.HideLanguageColumnInVersionGadget ? "hidden" : "visible";
            sb.Replace(nameof(cmsEdit.HideLanguageColumnInVersionGadget) + "Visibility", hideLanguageColumnInVersionGadgetVisibility);
            sb.Replace(nameof(cmsEdit.HideLanguageColumnInVersionGadget), hideLanguageColumnInVersionGadget);
            sb.Replace(nameof(cmsEdit.ContentTitleColor), cmsEdit.ContentTitleColor);
            sb.Replace(nameof(cmsEdit.ContentCreationBorderColor), cmsEdit.ContentCreationBorderColor);
            sb.Replace(nameof(cmsEdit.ContentCreationBackgroundColor), cmsEdit.ContentCreationBackgroundColor);
            sb.Replace(nameof(cmsEdit.PageTreeSelectedContentBorderColor), cmsEdit.PageTreeSelectedContentBorderColor);
            if (cmsEdit.ActiveProjectBarBackgroundColor.Is())
            {
                try
                {
                    sb.Replace(nameof(cmsEdit.ActiveProjectBarBackgroundColor) + "Border", cmsEdit.ActiveProjectBarBackgroundColor.HexDarkenOrLighten(auto: true));
                }
                catch
                {
                }
                sb.Replace(nameof(cmsEdit.ActiveProjectBarBackgroundColor), cmsEdit.ActiveProjectBarBackgroundColor);

            }
            AppendCustomPageTreeIcons(sb);
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        StylesheetCache = new FileContentResult(bytes, "text/css");

        return StylesheetCache;
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

                sb.Append("{background-image: url(" + url + ");}");
            }
        }
    }
}
