using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms;

public partial class ParentLinkReferenceController : Controller
{
    const string CurrentFolder = "Cms/ParentLinkReference";

    static int ClientCacheSeconds = 43200;

    static Assembly _CurrentAssembly;

    static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
        (_CurrentAssembly = Assembly.GetExecutingAssembly());

    static ActionResult ScriptCache;
    static ActionResult HtmlCache;

    public ActionResult Html()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

        if (HtmlCache != null) return HtmlCache;

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        var sb = new StringBuilder("");

        sb.Append(Net.Assemblies.GetEmbeddedResource(CurrentFolder, "ParentLinkReference.html", CurrentAssembly));

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        HtmlCache = new FileContentResult(bytes, "text/html");

        return HtmlCache;
    }

    public ActionResult Script()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

        if (ScriptCache != null) return ScriptCache;

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        var sb = new StringBuilder("");

        sb.Append(Net.Assemblies.GetEmbeddedResource(CurrentFolder, "ParentLinkReference.js", CurrentAssembly));

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        ScriptCache = new FileContentResult(bytes, "text/javascript");

        return ScriptCache;
    }
}
