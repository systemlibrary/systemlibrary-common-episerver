using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.FontAwesome;

partial class FontAwesomeController
{
    [Route("SystemLibrary/Common/Episerver/FontAwesome/BrandsIcon/{icon}")]
    public ActionResult BrandsIcon(string icon)
    {
        AddCacheControlHeader();

        return GetActionResult("Brands", icon);
    }
}
