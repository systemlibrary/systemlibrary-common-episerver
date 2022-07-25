using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms;

public partial class ParentLinkReferenceController : Controller
{
    const string CurrentFolder = "Cms/Properties/ParentLinkReference";

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

        var html = Net.Assemblies.GetEmbeddedResource(CurrentFolder, "ParentLinkReference.html", CurrentAssembly);

        if (html.IsNot())
            Log.Error(this.GetType().Name + " could not read html from folder " + CurrentFolder);

        var bytes = Encoding.UTF8.GetBytes(html);

        HtmlCache = new FileContentResult(bytes, "text/html");

        return HtmlCache;
    }

    public ActionResult Script()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

        if (ScriptCache != null)
            return ScriptCache;

        var script = Net.Assemblies.GetEmbeddedResource(CurrentFolder, "ParentLinkReference.js", CurrentAssembly);

        if (script.IsNot())
            Log.Error(this.GetType().Name + " could not read script from folder " + CurrentFolder);

        var bytes = Encoding.UTF8.GetBytes(script);

        ScriptCache = new FileContentResult(bytes, "text/javascript");

        return ScriptCache;
    }
}
