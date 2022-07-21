using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EPiServer.Core;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Attributes;

partial class ContentIconAttribute
{
    partial class ContentIconRouteModule
    {
        //Item1: Type is "StartPage"
        //Item2: ContentIcon is the attribute "on type"
        //Item3: string 1 is the css class for pagetree icons
        //Item4: string 2 is the custom relative icon url, as in: not using a FontAwesome Icon
        static internal List<Tuple<Type, ContentIconAttribute, string, string>> ContentDescriptorSettings = new List<Tuple<Type, ContentIconAttribute, string, string>>();

        static void LoadPageTypesWithIconAttribute()
        {
            var types = Assemblies.FindAllTypesInheritingWithAttribute<PageData, ContentIconAttribute>();

            if (types == null || types.Count() == 0) return;

            var options = from type in types
                              select new
                              {
                                  Type = type,
                                  Attribute = type.GetCustomAttribute<ContentIconAttribute>(),
                                  CssClass = "custom-common-episerver-page-tree-icon custom-common-episerver-page-tree-icon--" + type.Name.ToLower(),
                                  IconRelativeUrl = type.GetCustomAttribute<ContentIconAttribute>()?.IconRelativeUrl
                              };

            if (options == null) return;

            foreach (var o in options)
                ContentDescriptorSettings.Add(Tuple.Create(o.Type, o.Attribute, o.CssClass, o.IconRelativeUrl));
        }
    }
}
