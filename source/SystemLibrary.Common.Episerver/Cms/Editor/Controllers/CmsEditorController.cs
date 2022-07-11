using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms.Editor.Controllers
{
    public class CmsEditorController : Controller
    {
        static int ClientCacheLifespanSeconds = 14400;

        static Assembly _CurrentAssembly;

        static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
            (_CurrentAssembly = Assembly.GetExecutingAssembly());

        static FileContentResult Cache;

        public ActionResult Styles()
        {
            if (Cache != null) return Cache;

            Response.Headers.Clear();

            Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheLifespanSeconds);

            var cssFolder = "Cms/Editor/Css";

            var embeddedCss = Net.Assemblies.GetEmbeddedResource(cssFolder, "editor.min.css", CurrentAssembly);

            embeddedCss = embeddedCss.Replace("COMPANYCOLOR", AppSettings.Current.SystemLibraryCommonEpiserver.EditMode.CompanyColor ?? "#B84D94");

            var bytes = Encoding.UTF8.GetBytes(embeddedCss);

            Cache = new FileContentResult(bytes, "text/css");

            return Cache;
        }
    }
}
