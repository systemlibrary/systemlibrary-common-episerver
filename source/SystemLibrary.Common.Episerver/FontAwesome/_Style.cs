using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.FontAwesome;

partial class FontAwesomeController
{
    static FileContentResult FontAwesomeCache;

    public ActionResult Style()
    {
        AddCacheControlHeader();

        if (FontAwesomeCache != null) return FontAwesomeCache;

        var css = FontAwesomeLoader.FontAwesomeBundledMinCss;

        var bytes = Encoding.UTF8.GetBytes(css);

        FontAwesomeCache = new FileContentResult(bytes, "text/css");

        return FontAwesomeCache;
    }
}
