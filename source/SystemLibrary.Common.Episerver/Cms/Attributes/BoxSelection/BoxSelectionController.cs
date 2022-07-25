using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public partial class BoxSelectionController : Controller
{
    const string CurrentFolder = "Cms/Attributes/BoxSelection";

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

        var css = Net.Assemblies.GetEmbeddedResource(CurrentFolder, "BoxSelection.css", CurrentAssembly);

        if (css.IsNot())
            Log.Error(this.GetType().Name + " could not read script from folder " + CurrentFolder);

        var bytes = Encoding.UTF8.GetBytes(css);

        StyleCache = new FileContentResult(bytes, "text/css");

        return StyleCache;
    }

    public ActionResult Script()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

        if (ScriptCache != null) return ScriptCache;

        var css = Net.Assemblies.GetEmbeddedResource(CurrentFolder, "BoxSelection.js", CurrentAssembly);
        
        if (css.IsNot())
            Log.Error(this.GetType().Name + " could not read script from folder " + CurrentFolder);

        var bytes = Encoding.UTF8.GetBytes(css);

        ScriptCache = new FileContentResult(bytes, "text/javascript");

        return ScriptCache;
    }
}
