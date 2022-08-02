using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

public partial class JsonEditController : BaseController
{
    const string CurrentFolder = "Cms/Attributes/JsonEdit";

    static ActionResult ScriptCache;
    static ActionResult HtmlCache;
    static ActionResult EditorHtmlCache;

    static ActionResult EditorScriptCache;
    static ActionResult EditorStyleCache;

    public ActionResult Html()
    {
        AddCacheHeaders();

        if (HtmlCache != null) return HtmlCache;

        var html = GetEmbeddedResource(CurrentFolder, "JsonEdit.html");

        return (HtmlCache = GetFileContentResult(html, "text/html"));
    }
     public ActionResult Script()
    {
        AddCacheHeaders();

        if (ScriptCache != null) return ScriptCache;

        var script = GetEmbeddedResource(CurrentFolder, "JsonEdit.js");

        return (ScriptCache = GetFileContentResult(script, "text/javascript"));
    }

    public ActionResult EditorHtml()
    {
        AddCacheHeaders();

        if (EditorHtmlCache != null) return EditorHtmlCache;

        var editor = GetEmbeddedResource(CurrentFolder, "JsonEditor.html");

        return (EditorHtmlCache = GetFileContentResult(editor, "text/html"));
    }

    public ActionResult EditorScript()
    {
        AddCacheHeaders();

        if (EditorScriptCache != null) return EditorScriptCache;

        var editorScript = GetEmbeddedResource(CurrentFolder, "JsonEditor.js");

        return (EditorScriptCache = GetFileContentResult(editorScript, "text/javascript"));
    }

    public ActionResult EditorStyle()
    {
        AddCacheHeaders();

        if (EditorStyleCache != null) return EditorStyleCache;

        var editorStyle = GetEmbeddedResource(CurrentFolder, "JsonEditor.css");

        return (EditorStyleCache = GetFileContentResult(editorStyle, "text/css"));
    }

   
}
