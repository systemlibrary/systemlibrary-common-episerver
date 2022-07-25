using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.FontAwesome;

partial class FontAwesomeController
{
    [Route("SystemLibrary/Common/Episerver/FontAwesome/RegularIcon/{icon}")]
    public ActionResult RegularIcon(string icon)
    {
        AddCacheControlHeader();

        return GetActionResult("Regular", icon);
    }
}
