using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Attributes
{
    partial class ContentIconAttributeController
    {
        [Route("SystemLibrary/Common/Episerver/ContentIconAttribute/BrandsIcon/{icon}")]
        public ActionResult BrandsIcon(string icon)
        {
            AddCacheControlHeader();

            return GetActionResult("Brands", icon);
        }
    }
}
