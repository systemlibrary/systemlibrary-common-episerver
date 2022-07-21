using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms
{
    public partial class CmsEditController : Controller
    {
        const string CmsEditFolder = "Cms/CmsEdit";

        static int ClientCacheSeconds = 43200;

        static Assembly _CurrentAssembly;

        static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
            (_CurrentAssembly = Assembly.GetExecutingAssembly());

        static FileContentResult StylesheetCache;
        static FileContentResult ScriptCache;

        public ActionResult Stylesheet()
        {
            if (Response.Headers.ContainsKey("Cache-Control"))
                Response.Headers.Remove("Cache-Control");

            Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

            if (StylesheetCache != null) return StylesheetCache;

            var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

            StringBuilder sb = new StringBuilder(Net.Assemblies.GetEmbeddedResource(CmsEditFolder, "CmsEditStylesheet.css", CurrentAssembly));

            var hideLanguageColumnInVersionGadget = cmsEdit.HideLanguageColumnInVersionGadget ? "0px" : "";
            sb.Replace(nameof(cmsEdit.HideLanguageColumnInVersionGadget), hideLanguageColumnInVersionGadget);
            sb.Replace(nameof(cmsEdit.ContentTitleColor), cmsEdit.ContentTitleColor);
            sb.Replace(nameof(cmsEdit.ContentCreationBorderColor), cmsEdit.ContentCreationBorderColor);
            sb.Replace(nameof(cmsEdit.ContentCreationBackgroundColor), cmsEdit.ContentCreationBackgroundColor);
            sb.Replace(nameof(cmsEdit.PageTreeSelectedContentBorderColor), cmsEdit.PageTreeSelectedContentBorderColor);
            sb.Replace(nameof(cmsEdit.ActiveProjectBarBackgroundColor), cmsEdit.ActiveProjectBarBackgroundColor);

            AppendCustomPageTreeIcons(sb);

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            StylesheetCache = new FileContentResult(bytes, "text/css");

            return StylesheetCache;
        }

        public ActionResult Script()
        {
            if (Response.Headers.ContainsKey("Cache-Control"))
                Response.Headers.Remove("Cache-Control");

            Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

            if (ScriptCache != null) return ScriptCache;

            var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

            var sb = new StringBuilder("");

            if (cmsEdit.ActiveProjectBarBackgroundColor.Is())
            {
                sb.Append(Net.Assemblies.GetEmbeddedResource(CmsEditFolder, "CmsEditScript.js", CurrentAssembly));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            ScriptCache = new FileContentResult(bytes, "text/javascript");

            return ScriptCache;
        }
    }
}
