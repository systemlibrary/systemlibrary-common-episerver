﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Attributes
{
    public partial class ContentIconAttributeController : Controller
    {
        static int ClientCacheSeconds = 86400;

        static Assembly _CurrentAssembly;

        static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
            (_CurrentAssembly = Assembly.GetExecutingAssembly());

        static ConcurrentDictionary<string, ActionResult> CacheActionResults;

        static ContentIconAttributeController()
        {
            CacheActionResults = new ConcurrentDictionary<string, ActionResult>();
        }

        static ActionResult GetActionResult(string folder, string icon)
        {
            var cacheKey = folder + icon;
            if (CacheActionResults.TryGetValue(cacheKey, out ActionResult cached))
                return cached;

            var bytes = GetEmbeddedIcon(folder, icon);

            if(bytes == null)
                cached = new EmptyResult();
            else
                cached = new FileContentResult(bytes, "image/svg+xml");

            CacheActionResults.TryAdd(cacheKey, cached);

            return cached;
        }

        static byte[] GetEmbeddedIcon(string iconsFolder, string name)
        {
            if (name.IsNot()) return null;

            try
            {
                var image = Net.Assemblies.GetEmbeddedResource("Attributes/ContentIconAttribute/FontAwesome/Icons/" + iconsFolder, name, CurrentAssembly);
                if (image.IsNot())
                {
                    Log.Warning("FontAwesome icon not found: " + name);
                    return null;
                }

                return Encoding.UTF8.GetBytes(image);
            }
            catch (Exception ex)
            {
                Log.Warning("FontAwesome icon not loaded: " + name + ", please chose another one. Message: " + ex.Message);
                return null;
            }
        }

        [Route("SystemLibrary/Common/Episerver/WebFonts/{filePath?}")]
        public ActionResult WebFonts(string filePath)
        {
            Dump.Write("WebFonts is invoked!");
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


        void AddCacheControlHeader()
        {
            if (Response.Headers.ContainsKey("Cache-Control"))
                Response.Headers.Remove("Cache-Control");

            Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);
        }
    }
}
