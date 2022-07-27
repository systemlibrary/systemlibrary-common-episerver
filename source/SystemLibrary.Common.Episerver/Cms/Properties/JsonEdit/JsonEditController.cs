using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Properties;

namespace SystemLibrary.Common.Episerver.Cms;

public partial class JsonEditController : BasePropertyController
{
    const string CurrentFolder = "Cms/Properties/JsonEdit";

    static ActionResult ScriptCache;
    static ActionResult HtmlCache;

    public ActionResult Html()
    {
        AddCacheHeaders();

        if (HtmlCache != null) return HtmlCache;

        var html = Net.Assemblies.GetEmbeddedResource(CurrentFolder, "JsonEdit.html", CurrentAssembly);

        if (html.IsNot())
            Log.Error(this.GetType().Name + " could not read Html from " + CurrentFolder);

        var bytes = Encoding.UTF8.GetBytes(html);

        HtmlCache = new FileContentResult(bytes, "text/html");

        return HtmlCache;
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

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
