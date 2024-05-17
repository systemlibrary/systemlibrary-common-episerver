using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Abstract;

namespace SystemLibrary.Common.Episerver.Attributes;

public partial class JsonEditController : BaseController
{
    const string CurrentFolder = "Cms/Attributes/JsonEdit";

    static ActionResult ScriptCache;
    static ActionResult HtmlCache;
    static ActionResult EditorHtmlCache;

    static ActionResult EditorScriptCache;
    static ActionResult EditorStyleCache;

    static ActionResult EasyEditorStyleCache;
    static ActionResult EasyEditorScriptCache;

    public ActionResult EasyEditorStyle()
    {
        AddCacheHeaders();

        if (IsCached(EasyEditorStyleCache)) return EasyEditorStyleCache;

        var html = GetEmbeddedResource(CurrentFolder, "EasyEditorStyle.css");

        return (EasyEditorStyleCache = GetFileContentResult(html, "text/css"));
    }

    public ActionResult EasyEditorScript()
    {
        AddCacheHeaders();

        if (IsCached(EasyEditorScriptCache)) return EasyEditorScriptCache;

        var html = GetEmbeddedResource(CurrentFolder, "EasyEditorScript.js");

        return (EasyEditorScriptCache = GetFileContentResult(html, "text/javascript"));
    }

    public ActionResult Html()
    {
        AddCacheHeaders();

        if (IsCached(HtmlCache)) return HtmlCache;

        var html = GetEmbeddedResource(CurrentFolder, "JsonEdit.html");

        return (HtmlCache = GetFileContentResult(html, "text/html"));
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if (IsCached(ScriptCache)) return ScriptCache;

        var script = GetEmbeddedResource(CurrentFolder, "JsonEdit.js");

        return (ScriptCache = GetFileContentResult(script, "text/javascript"));
    }

    public ActionResult EditorHtml()
    {
        AddCacheHeaders();

        if (IsCached(EditorHtmlCache)) return EditorHtmlCache;

        var editor = GetEmbeddedResource(CurrentFolder, "JsonEditor.html");

        return (EditorHtmlCache = GetFileContentResult(editor, "text/html"));
    }

    public ActionResult EditorScript()
    {
        AddCacheHeaders();

        if (IsCached(EditorScriptCache)) return EditorScriptCache;

        var editorScript = GetEmbeddedResource(CurrentFolder, "JsonEditor.js");

        return (EditorScriptCache = GetFileContentResult(editorScript, "text/javascript"));
    }

    public ActionResult EditorStyle()
    {
        AddCacheHeaders();

        if (IsCached(EditorStyleCache)) return EditorStyleCache;

        var editorStyle = GetEmbeddedResource(CurrentFolder, "JsonEditor.css");

        return (EditorStyleCache = GetFileContentResult(editorStyle, "text/css"));
    }

   
}
