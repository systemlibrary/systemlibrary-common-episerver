using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SystemLibrary.Common.Episerver.Attributes
{
    partial class ContentIconAttribute
    {
        partial class ContentIconRouteModule
        {
            static Dictionary<string, ContentIconAttribute> GetPagesWithContentTypeIcons()
            {
                var pageTypes = FindAllPagesWithContentIconAttribute();

                if (pageTypes == null) return new Dictionary<string, ContentIconAttribute>();

                return (from pageType in pageTypes
                        select new
                        {
                            pageType.Name,
                            ContentIcon = pageType.GetCustomAttribute<ContentIconAttribute>()
                        }).ToDictionary(key => key.Name, value => value.ContentIcon);
            }

         
        }
    }
}
