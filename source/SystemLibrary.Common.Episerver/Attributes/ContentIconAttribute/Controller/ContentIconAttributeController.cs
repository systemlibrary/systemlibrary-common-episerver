using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Attributes
{
    public class ContentIconAttributeController : Controller
    {
        static int ClientCacheLifespanSeconds = 14400;
        const string CacheKeyPrefix = "SystemLibrary.Common.Episerver.ContentIconAttributeController";

        static Assembly _CurrentAssembly;

        static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
            (_CurrentAssembly = Assembly.GetExecutingAssembly());

        static IDictionary<string, ActionResult> CacheResults;

        static ContentIconAttributeController()
        {
            CacheResults = new Dictionary<string, ActionResult>();
        }

        public ActionResult FontAwesome()
        {
            Response.Headers.Clear();

            Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheLifespanSeconds);

            var embeddedCss = ContentIconAttribute.FontAwesomeIconsLoader.FontAwesomeBundledMinCss;
            var bytes = Encoding.UTF8.GetBytes(embeddedCss);

            return new FileContentResult(bytes, "text/css");
        }

        [Route("SystemLibrary/Common/Episerver/ContentIcons/CustomIcon/{iconPath}")]
        public ActionResult CustomIcon(string iconPath)
        {
            Dump.Write("Custom icon??? " + iconPath);
            iconPath = iconPath.Replace("___", "/");

            return Redirect(iconPath);
        }

        [Route("SystemLibrary/Common/Episerver/ContentIcons/RegularIcon/{icon}")]
        public ActionResult RegularIcon(string icon)
        {
            return GetActionResult("Regular", icon);
        }

        [Route("SystemLibrary/Common/Episerver/ContentIcons/SolidIcon/{icon}")]
        public ActionResult SolidIcon(string icon)
        {
            return GetActionResult("Solid", icon);
        }

        [Route("SystemLibrary/Common/Episerver/ContentIcons/BrandsIcon/{icon}")]
        public ActionResult BrandsIcon(string icon)
        {
            return GetActionResult("Brands", icon);
        }

        static ActionResult GetActionResult(string folder, string icon)
        {
            var cacheKey = CacheKeyPrefix + folder + icon;
            if (CacheResults.TryGetValue(cacheKey, out ActionResult cached))
                return cached;

            var bytes = GetEmbeddedIcon(folder, icon);

            if(bytes == null)
                cached = new EmptyResult();
            else
                cached = new FileContentResult(bytes, "image/svg+xml");

            CacheResults.TryAdd(CacheKeyPrefix + folder + icon, cached);
            return cached;
        }

        static byte[] GetEmbeddedIcon(string iconsFolder, string name)
        {
            if (name.IsNot()) return null;

            try
            {
                var image = Net.Assemblies.GetEmbeddedResource("Attributes/ContentIconAttribute/Icons/" + iconsFolder, name, CurrentAssembly);

                return Encoding.UTF8.GetBytes(image);
            }
            catch (Exception ex)
            {
                Log.Warning("Regular fontawesome icon not found: " + name + ", " + ex.Message);
                return null;
            }
        }

        [Route("SystemLibrary/Common/Episerver/WebFonts/{filePath?}")]
        public ActionResult WebFonts(string filePath)
        {
            if (filePath.IsNot()) return new EmptyResult();

            if (filePath.Contains("olid"))
            {
                return new FileContentResult(ContentIconAttribute.FontAwesomeIconsLoader.Solid900Woff2, "font/woff2");
            }
            if (filePath.Contains("rands"))
            {
                return new FileContentResult(ContentIconAttribute.FontAwesomeIconsLoader.Brands400Woff2, "font/woff2");
            }

            return new FileContentResult(ContentIconAttribute.FontAwesomeIconsLoader.Regular400Woff2, "font/woff2");
        }


    }
}
