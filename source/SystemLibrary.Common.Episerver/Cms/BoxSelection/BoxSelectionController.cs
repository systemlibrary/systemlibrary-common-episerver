using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms;

public partial class BoxSelectionController : Controller
{
    const string CurrentFolder = "Cms/BoxSelection";

    static int ClientCacheSeconds = 43200;

    static Assembly _CurrentAssembly;

    static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
        (_CurrentAssembly = Assembly.GetExecutingAssembly());

    static ActionResult ScriptCache;
    static ActionResult StyleCache;

    public ActionResult Style()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

        if (StyleCache != null) return StyleCache;

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        var sb = new StringBuilder("");

        sb.Append(Net.Assemblies.GetEmbeddedResource(CurrentFolder, "BoxSelection.css", CurrentAssembly));

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        StyleCache = new FileContentResult(bytes, "text/css");

        return StyleCache;
    }

    public ActionResult Script()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

        if (ScriptCache != null) return ScriptCache;

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        var sb = new StringBuilder("");

        sb.Append(Net.Assemblies.GetEmbeddedResource(CurrentFolder, "BoxSelection.js", CurrentAssembly));

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        ScriptCache = new FileContentResult(bytes, "text/javascript");

        return ScriptCache;
    }
}
