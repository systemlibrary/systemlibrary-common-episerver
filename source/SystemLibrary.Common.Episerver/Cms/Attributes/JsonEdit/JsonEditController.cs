using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public partial class JsonEditController : BaseCmsController
{
    const string CurrentFolder = "Cms/Attributes/JsonEdit";

    static ActionResult ScriptCache;
    static ActionResult HtmlCache;
    static ActionResult EditorCache;

    public ActionResult Html()
    {
        AddCacheHeaders();

        if (HtmlCache != null) return HtmlCache;

        var html = GetEmbeddedResource(CurrentFolder, "JsonEdit.html");

        return (HtmlCache = GetFileContentResult(html, "text/html"));
    }

    public ActionResult Editor()
    {
        AddCacheHeaders();

        if (EditorCache != null) return EditorCache;

        var editor = GetEmbeddedResource(CurrentFolder, "JsonEditor.html");

        return (EditorCache = GetFileContentResult(editor, "text/html"));
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if (ScriptCache != null) return ScriptCache;

        var script = GetEmbeddedResource(CurrentFolder, "JsonEdit.js");

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        if (cmsEdit.Enabled)
        {
            script.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor) + "Darkened", cmsEdit.MessagePropertyBackgroundColor.HexDarkenOrLighten(0.41, false));
            script.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor), cmsEdit.MessagePropertyBackgroundColor);
        }

        return (ScriptCache = GetFileContentResult(script, "text/javascript"));
    }
}
