using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Cms.Abstract;

namespace SystemLibrary.Common.Episerver.Cms.Properties;

public partial class MessageController : BaseController
{
    const string CurrentFolder = "Cms/Properties/Message";

    static FileContentResult HtmlCache;
    static FileContentResult ScriptCache;
    static FileContentResult StyleCache;

    public ActionResult Html()
    {
        AddCacheHeaders();

        if (IsCached(HtmlCache)) return HtmlCache;

        var data = GetEmbeddedResource(CurrentFolder, "Message.html");

        return HtmlCache = GetFileContentResult(data, "text/html");
    }

    public ActionResult Script()
    {
        AddCacheHeaders();

        if (IsCached(ScriptCache)) return ScriptCache;

        var data = GetEmbeddedResource(CurrentFolder, "Message.js");

        var edit = AppSettings.Current.Edit;

        data.Replace(nameof(edit.Properties.Message.TextColor), edit.Properties.Message.TextColor);

        return ScriptCache = GetFileContentResult(data, "text/javascript");
    }

    public ActionResult Style()
    {
        AddCacheHeaders();

        if (IsCached(StyleCache)) return StyleCache;

        var data = GetEmbeddedResource(CurrentFolder, "Message.css");

        var edit = AppSettings.Current.Edit;

        data.Replace(nameof(edit.Properties.Message.TextColor), edit.Properties.Message.TextColor);
        data.Replace(nameof(edit.Properties.Message.BackgroundColor), edit.Properties.Message.BackgroundColor);

        return StyleCache = GetFileContentResult(data, "text/css");
    }
}
