using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Attributes
{
    public class ContentIconAttributeController : Controller
    {
        static int ClientCacheLifespanSeconds = 7200;

        static Assembly _CurrentAssembly;

        static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
            (_CurrentAssembly = Assembly.GetExecutingAssembly());

        static IDictionary<string, byte[]> IconCache;

        static ContentIconAttributeController()
        {
            IconCache = new Dictionary<string, byte[]>();
        }

        public ActionResult FontAwesome()
        {
            Response.Headers.Clear();

            Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheLifespanSeconds);

            var bundledCss = ContentIconAttribute.FontAwesomeIconsLoader.FontAwesomeBundledMinCss;
            var bytes = Encoding.UTF8.GetBytes(bundledCss);

            return new FileContentResult(bytes, "text/css");
        }

        [Route("SystemLibrary/ContentIcons/RegularIcon/{name}")]
        public ActionResult RegularIcon(string name)
        {
            var bytes = GetEmbeddedIcon("Regular", name);

            if (bytes == null) return new EmptyResult();

            return new FileContentResult(bytes, "image/svg+xml");
        }

        [Route("SystemLibrary/ContentIcons/SolidIcon/{name}")]
        public ActionResult SolidIcon(string name)
        {
            var bytes = GetEmbeddedIcon("Solid", name);

            if (bytes == null) return new EmptyResult();

            return new FileContentResult(bytes, "image/svg+xml");
        }

        [Route("SystemLibrary/ContentIcons/BrandsIcon/{name}")]
        public ActionResult BrandsIcon(string name)
        {
            var bytes = GetEmbeddedIcon("Brands", name);

            if(bytes == null) return new EmptyResult();

            return new FileContentResult(bytes, "image/svg+xml");
        }

        static byte[] GetEmbeddedIcon(string iconsFolder, string name)
        {
            if (name.IsNot()) return null;

            var cacheKey = iconsFolder + name;
            if (IconCache.TryGetValue(cacheKey, out var icon))
                return icon;

            try
            {
                var image = Net.Assemblies.GetEmbeddedResource("Attributes/ContentIconAttribute/Icons/" + iconsFolder, name, CurrentAssembly);

                var bytes = Encoding.UTF8.GetBytes(image);

                IconCache.TryAdd(cacheKey, bytes);

                return bytes;
                
            }
            catch (Exception ex)
            {
                Log.Warning("Regular fontawesome icon not found: " + name + ", " + ex.Message);
                return null;
            }
        }

        [Route("SystemLibrary/WebFonts/{filePath?}")]
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
