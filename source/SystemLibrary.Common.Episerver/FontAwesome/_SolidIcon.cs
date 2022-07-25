using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.FontAwesome;

partial class FontAwesomeController
{
    [Route("SystemLibrary/Common/Episerver/FontAwesome/SolidIcon/{icon}")]
    public ActionResult SolidIcon(string icon)
    {
        AddCacheControlHeader();

        return GetActionResult("Solid", icon);
    }
}
