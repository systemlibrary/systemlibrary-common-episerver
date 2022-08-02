using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;

namespace SystemLibrary.Common.Episerver.Cms.Properties;

public partial class ParentLinkReferenceController : BaseController
{
    const string CurrentFolder = "Cms/Properties/ParentLinkReference";

    static ActionResult ScriptCache;
    static ActionResult HtmlCache;
    
    public ActionResult Html()
    {
        AddCacheHeaders();

        if (HtmlCache != null) return HtmlCache;

        var html = GetEmbeddedResource(CurrentFolder, "ParentLinkReference.html");

        return (HtmlCache = GetFileContentResult(html, "text/html"));
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if (ScriptCache != null) return ScriptCache;

        var script = GetEmbeddedResource(CurrentFolder, "ParentLinkReference.js");

        return (ScriptCache = GetFileContentResult(script, "text/javascript"));
    }
}
