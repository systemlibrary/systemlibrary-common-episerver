using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms
{
    public partial class CmsEditController : Controller
    {
        const string CmsEditFolder = "Cms/CmsEdit";

        static int ClientCacheSeconds = 43200;

        static Assembly _CurrentAssembly;

        static Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
            (_CurrentAssembly = Assembly.GetExecutingAssembly());

        static FileContentResult StylesheetCache;

        public ActionResult Stylesheet()
        {
            if (Response.Headers.ContainsKey("Cache-Control"))
                Response.Headers.Remove("Cache-Control");

            Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);

            if (StylesheetCache != null) return StylesheetCache;

            var cmsEdit = AppSettings.Current.SystemLibraryCommonEpiserver.CmsEdit;

            StringBuilder sb = new StringBuilder(Net.Assemblies.GetEmbeddedResource(CmsEditFolder, "CmsEditStylesheet.css", CurrentAssembly));

            var hideLanguageColumnInVersionGadget = cmsEdit.HideLanguageColumnInVersionGadget ? "0px" : "";
            var hideLanguageColumnInVersionGadgetVisibility = cmsEdit.HideLanguageColumnInVersionGadget ? "hidden" : "visible";
            sb.Replace(nameof(cmsEdit.HideLanguageColumnInVersionGadget) + "Visibility", hideLanguageColumnInVersionGadgetVisibility);
            sb.Replace(nameof(cmsEdit.HideLanguageColumnInVersionGadget), hideLanguageColumnInVersionGadget);
            sb.Replace(nameof(cmsEdit.ContentTitleColor), cmsEdit.ContentTitleColor);
            sb.Replace(nameof(cmsEdit.ContentCreationBorderColor), cmsEdit.ContentCreationBorderColor);
            sb.Replace(nameof(cmsEdit.ContentCreationBackgroundColor), cmsEdit.ContentCreationBackgroundColor);
            sb.Replace(nameof(cmsEdit.PageTreeSelectedContentBorderColor), cmsEdit.PageTreeSelectedContentBorderColor);
            if (cmsEdit.ActiveProjectBarBackgroundColor.Is())
            {
                try
                {
                    sb.Replace(nameof(cmsEdit.ActiveProjectBarBackgroundColor) + "Border", Darken(cmsEdit.ActiveProjectBarBackgroundColor));
                }
                catch
                {
                }
                sb.Replace(nameof(cmsEdit.ActiveProjectBarBackgroundColor), cmsEdit.ActiveProjectBarBackgroundColor);

            }

            AppendCustomPageTreeIcons(sb);

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            StylesheetCache = new FileContentResult(bytes, "text/css");

            return StylesheetCache;
        }

        static string Darken(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            int partLength = hex.Length == 6 ? 2 : 1;

            var parts = SplitHex(hex, partLength);

            var darkenedColor = "";
            var factor = 0.31;
            foreach (var part in parts)
            {
                var color = Convert.ToInt32(part, 16);
                darkenedColor += Convert.ToInt32(color * factor).ToString("X");
            }

            return "#" + darkenedColor;
        }

        static IEnumerable<string> SplitHex(string hex, int partLength)
        {
            for (var i = 0; i < hex.Length; i += partLength)
                yield return hex.Substring(i, Math.Min(partLength, hex.Length - i));
        }
    }
}
