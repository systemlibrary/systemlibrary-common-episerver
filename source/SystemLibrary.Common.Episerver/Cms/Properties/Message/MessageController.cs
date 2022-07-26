using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms;

public partial class MessageController : Controller
{
    const string CurrentFolder = "Cms/Properties/Message";

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

        var html = new StringBuilder(Net.Assemblies.GetEmbeddedResource(CurrentFolder, "Message.html", CurrentAssembly) ?? "");

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        if (cmsEdit.Enabled)
        {
            html.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor), cmsEdit.MessagePropertyBackgroundColor);
        }
        if(html.Length < 1)
            Log.Error(this.GetType().Name + " could not read html from folder " + CurrentFolder);

        var bytes = Encoding.UTF8.GetBytes(html.ToString());

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

        var script = new StringBuilder(Net.Assemblies.GetEmbeddedResource(CurrentFolder, "Message.js", CurrentAssembly) ?? "");

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        if (cmsEdit.Enabled)
        {
            script.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor) + "Darkened", cmsEdit.MessagePropertyBackgroundColor.HexDarkenOrLighten(0.41, false));
            script.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor), cmsEdit.MessagePropertyBackgroundColor);
        }
        if (script.Length < 1)
            Log.Error(this.GetType().Name + " could not read script from folder " + CurrentFolder);

        var bytes = Encoding.UTF8.GetBytes(script.ToString());

        ScriptCache = new FileContentResult(bytes, "text/javascript");

        return ScriptCache;
    }
}
