using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Episerver.Initialize
{
    internal class EpiViewLocations : IViewLocationExpander
    {
        static string[] GetViews() => new string[] {
                "~/Views/Shared/{0}.cshtml", //Required by Episerver
        };

        static string[] GetViewsForPages() => new string[] {
            "~/Content/Pages/{1}/{0}.cshtml",
            "~/Content/Pages/{1}/Views/{0}.cshtml"
        };

        static string[] GetViewsForBlocks() => new string[] {
            "~/Content/Blocks/{0}.cshtml",
            "~/Content/Blocks/{0}/Index.cshtml",
            "~/Content/Blocks/{1}/{0}.cshtml",
            "~/Content/Blocks/{0}/{0}.cshtml",
            "~/Content/Blocks/{1}/Views/{0}.cshtml",
            "~/Content/Blocks/{1}/Views/Index.cshtml",
            "~/Views/Blocks/{0}.cshtml"
        };

        static string[] GetViewsForComponents() => new string[]
        {
        };

        static string[] _AllViews;

        static string[] AllViews = (_AllViews != null) ? _AllViews :
            (_AllViews = GetViewsForComponents()
            .Concat(GetViewsForPages())
            .Concat(GetViewsForBlocks())
            .Concat(GetViews()).ToArray());

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (viewLocations == null)
                throw new ArgumentNullException(nameof(viewLocations));

            return AllViews;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
