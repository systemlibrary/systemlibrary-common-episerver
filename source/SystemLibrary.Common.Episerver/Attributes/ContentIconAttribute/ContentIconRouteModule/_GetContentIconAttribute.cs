using System.Collections.Generic;
using System.Linq;

namespace SystemLibrary.Common.Episerver.Attributes;

partial class ContentIconAttribute
{
    partial class ContentIconRouteModule
    {
        static Dictionary<string, ContentIconAttribute> _ContentTypesWithContentIconAttribute;

        static Dictionary<string, ContentIconAttribute> ContentTypesWithContentIconAttribute =>
            _ContentTypesWithContentIconAttribute != null ? _ContentTypesWithContentIconAttribute :
            (_ContentTypesWithContentIconAttribute = GetPagesWithContentTypeIcons());

        static ContentIconAttribute GetContentIconAttribute(string name)
        {
            if (name.IsNot()) return null;

            if (!ContentTypesWithContentIconAttribute.ContainsKey(name)) return null;

            return ContentTypesWithContentIconAttribute.FirstOrDefault(x => x.Key == name).Value;
        }
    }
}
