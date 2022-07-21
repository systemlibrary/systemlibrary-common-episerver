using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Attributes
{
    partial class ContentIconAttributeController
    {
        [Route("SystemLibrary/Common/Episerver/ContentIconAttribute/RegularIcon/{icon}")]
        public ActionResult RegularIcon(string icon)
        {
            AddCacheControlHeader();

            return GetActionResult("Regular", icon);
        }
    }
}
