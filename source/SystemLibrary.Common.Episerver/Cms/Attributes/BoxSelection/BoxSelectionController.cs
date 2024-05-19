using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Abstract;

namespace SystemLibrary.Common.Episerver.Attributes;

internal partial class BoxSelectionController : BaseController
{
    const string CurrentFolder = "Cms/Attributes/BoxSelection";

    static ActionResult ScriptCache;
    static ActionResult StyleCache;

    public ActionResult Style()
    {
        AddCacheHeaders();

        if (IsCached(StyleCache)) return StyleCache;

        var css = GetEmbeddedResource(CurrentFolder, "BoxSelection.css");

        return (StyleCache = GetFileContentResult(css, "text/css"));
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if(IsCached(ScriptCache)) return ScriptCache;

        var css = GetEmbeddedResource(CurrentFolder, "BoxSelection.js");

        return (ScriptCache = GetFileContentResult(css, "text/javascript"));
    }
}
