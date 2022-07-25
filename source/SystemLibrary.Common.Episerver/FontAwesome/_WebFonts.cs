
using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.FontAwesome;

partial class FontAwesomeController
{
    [Route("SystemLibrary/Common/Episerver/FontAwesome/Fonts/{filePath?}")]
    public ActionResult Fonts(string filePath)
    {
        if (filePath.IsNot()) return new EmptyResult();

        if (filePath.Contains("olid"))
        {
            return new FileContentResult(FontAwesomeLoader.Solid900Woff2, "font/woff2");
        }
        if (filePath.Contains("rands"))
        {
            return new FileContentResult(FontAwesomeLoader.Brands400Woff2, "font/woff2");
        }

        return new FileContentResult(FontAwesomeLoader.Regular400Woff2, "font/woff2");
    }

}
