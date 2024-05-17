using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Abstract;

namespace SystemLibrary.Common.Episerver.Attributes;

public partial class MultiDropdownSelectionController : BaseController
{
    const string CurrentFolder = "Cms/Attributes/MultiDropdownSelection";

    static ActionResult ScriptCache;
    static ActionResult StyleCache;
    static ActionResult HtmlCache;

    public ActionResult Style()
    {
        AddCacheHeaders();

        if (IsCached(StyleCache)) return StyleCache;

        var css = GetEmbeddedResource(CurrentFolder, "MultiDropdownSelection.css");

        return (StyleCache = GetFileContentResult(css, "text/css"));
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if (IsCached(ScriptCache)) return ScriptCache;

        var css = GetEmbeddedResource(CurrentFolder, "MultiDropdownSelection.js");

        return (ScriptCache = GetFileContentResult(css, "text/javascript"));
    }

    public ActionResult Html()
    {
        AddCacheHeaders();

        if (IsCached(HtmlCache)) return HtmlCache;

        var html = GetEmbeddedResource(CurrentFolder, "MultiDropdownSelection.html");

        return (HtmlCache = GetFileContentResult(html, "text/html"));
    }
}
