using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Attributes
{
    partial class ContentIconAttributeController
    {
        [Route("SystemLibrary/Common/Episerver/ContentIconAttribute/SolidIcon/{icon}")]
        public ActionResult SolidIcon(string icon)
        {
            AddCacheControlHeader();

            return GetActionResult("Solid", icon);
        }
    }
}
