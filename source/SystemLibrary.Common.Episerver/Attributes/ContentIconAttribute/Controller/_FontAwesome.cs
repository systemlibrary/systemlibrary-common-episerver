using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Attributes
{
    partial class ContentIconAttributeController
    {
        static FileContentResult FontAwesomeCache;

        public ActionResult FontAwesome()
        {
            AddCacheControlHeader();

            if (FontAwesomeCache != null) return FontAwesomeCache;

            var css = ContentIconAttribute.FontAwesomeIconsLoader.FontAwesomeBundledMinCss;

            var bytes = Encoding.UTF8.GetBytes(css);

            FontAwesomeCache = new FileContentResult(bytes, "text/css");

            return FontAwesomeCache;
        }
    }
}
