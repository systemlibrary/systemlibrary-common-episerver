using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;

namespace SystemLibrary.Common.Episerver.Cms.Properties;

public partial class MessageController : BaseController
{
    const string CurrentFolder = "Cms/Properties/Message";

    static FileContentResult HtmlCache;
    static FileContentResult ScriptCache;

    public ActionResult Html()
    {
        AddCacheHeaders();

        if (HtmlCache != null) return HtmlCache;

        var html = GetEmbeddedResource(CurrentFolder, "Message.html");

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        if (cmsEdit.Enabled)
        {
            html.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor), cmsEdit.MessagePropertyBackgroundColor);
        }

        return (HtmlCache = GetFileContentResult(html, "text/html"));
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if (ScriptCache != null) return ScriptCache;

        var script = GetEmbeddedResource(CurrentFolder, "Message.js");

        var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

        if (cmsEdit.Enabled)
        {
            script.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor) + "Darkened", cmsEdit.MessagePropertyBackgroundColor.HexDarkenOrLighten(0.41, false));
            script.Replace(nameof(cmsEdit.MessagePropertyBackgroundColor), cmsEdit.MessagePropertyBackgroundColor);
        }

        return (ScriptCache = GetFileContentResult(script, "text/javascript"));
    }
}
