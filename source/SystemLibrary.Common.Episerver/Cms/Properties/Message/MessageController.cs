using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Abstract;

namespace SystemLibrary.Common.Episerver.Properties;

internal partial class MessageController : BaseController
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

        var properties = AppSettings.Current.Properties;

        data.Replace(nameof(properties.Message.TextColor), properties.Message.TextColor);

        return ScriptCache = GetFileContentResult(data, "text/javascript");
    }

    public ActionResult Style()
    {
        AddCacheHeaders();

        if (IsCached(StyleCache)) return StyleCache;

        var data = GetEmbeddedResource(CurrentFolder, "Message.css");

        var properties = AppSettings.Current.Properties;

        data.Replace(nameof(properties.Message.TextColor), properties.Message.TextColor);
        data.Replace(nameof(properties.Message.BackgroundColor), properties.Message.BackgroundColor);

        return StyleCache = GetFileContentResult(data, "text/css");
    }
}
