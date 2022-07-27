using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public partial class BoxSelectionController : BaseCmsController
{
    const string CurrentFolder = "Cms/Attributes/BoxSelection";

    static ActionResult ScriptCache;
    static ActionResult StyleCache;

    public ActionResult Style()
    {
        AddCacheHeaders();

        if (StyleCache != null) return StyleCache;

        var css = GetEmbeddedResource(CurrentFolder, "BoxSelection.css");

        return (StyleCache = GetFileContentResult(css, "text/css"));
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if (ScriptCache != null) return ScriptCache;

        var css = GetEmbeddedResource(CurrentFolder, "BoxSelection.js");

        return (ScriptCache = GetFileContentResult(css, "text/javascript"));
    }
}
