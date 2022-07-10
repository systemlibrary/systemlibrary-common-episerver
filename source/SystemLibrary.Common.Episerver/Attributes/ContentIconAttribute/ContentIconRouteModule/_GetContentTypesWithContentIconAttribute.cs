using System;
using System.Collections.Generic;

using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Attributes;

partial class ContentIconAttribute
{
    partial class ContentIconRouteModule
    {
        static IEnumerable<Type> FindAllPagesWithContentIconAttribute()
        {
            return Net.Assemblies.FindAllTypesInheritingWithAttribute<PageData, ContentIconAttribute>();
        }
    }
}
