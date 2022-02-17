using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

using SystemLibrary.Common.Episerver.Initialize;

namespace SystemLibrary.Common.Episerver.Attributes
{
    partial class ContentIconAttribute
    {
        [InitializableModule]
        [ModuleDependency(typeof(EPiServer.Cms.Shell.InitializableModule))]
        internal class ContentIconRouteModule : InitModule
        {
            static bool IsInitialized;

            public override void Initialize(InitializationEngine context)
            {
                if (IsInitialized || context.HostType != HostType.WebApplication) return;
                IsInitialized = true;

                //TODO: Method is too long, too... 
                //make the module partial and split the functions out...
                var uiDescriptorRegistry = context.Locate?.Advanced?.GetInstance<UIDescriptorRegistry>();

                try
                {
                    if (uiDescriptorRegistry?.UIDescriptors == null) return;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return;
                }

                foreach (var uiDescriptor in uiDescriptorRegistry.UIDescriptors)
                {
                    if (uiDescriptor == null) continue;

                    var contentIconAttribute = GetContentIconAttribute(uiDescriptor.ForType.Name);

                    if (contentIconAttribute == null || contentIconAttribute.Value == null) continue;

                    var fontAwesomeIconName = contentIconAttribute.Value.GetType();

                    var iconName = contentIconAttribute.Value.ToString();
                    if (iconName.StartsWith("___"))
                        iconName = iconName.Substring(3);

                    var fontawesomeIcon = iconName.Replace("_", "-");

                    if (fontAwesomeIconName == typeof(FontAwesomeSolid))
                    {
                        uiDescriptor.IconClass = "dijitIcon dijitTreeIcon epi-iconObjectPage fas fa-" + fontawesomeIcon;
                    }
                    else if (fontAwesomeIconName == typeof(FontAwesomeBrands))
                    {
                        uiDescriptor.IconClass = "dijitIcon dijitTreeIcon epi-iconObjectPage fab fa-" + fontawesomeIcon;
                    }
                    else
                    {
                        uiDescriptor.IconClass = "dijitIcon dijitTreeIcon epi-iconObjectPage far fa-" + fontawesomeIcon;
                    }
                }
            }

            static ContentIconAttribute GetContentIconAttribute(string name)
            {
                if (!ContentTypesWithContentIconAttribute.ContainsKey(name)) return null;

                return ContentTypesWithContentIconAttribute.FirstOrDefault(x => x.Key == name).Value;
            }

            static Dictionary<string, ContentIconAttribute> GetPagesWithContentTypeIcons()
            {
                var pageTypes = GetContentTypesWithContentIconAttribute();

                if (pageTypes == null) return new Dictionary<string, ContentIconAttribute>();

                return (from pageType in pageTypes
                        select new
                        {
                            pageType.Name,
                            ContentIcon = pageType.GetCustomAttribute<ContentIconAttribute>()
                        }).ToDictionary(key => key.Name, value => value.ContentIcon);
            }

            static IEnumerable<Type> GetContentTypesWithContentIconAttribute()
            {
                return Net.Assemblies.FindAllTypesInheritingWithAttribute<PageData, ContentIconAttribute>();
            }

            static Dictionary<string, ContentIconAttribute> _ContentTypesWithContentIconAttribute;
            static Dictionary<string, ContentIconAttribute> ContentTypesWithContentIconAttribute =>
                _ContentTypesWithContentIconAttribute != null ? _ContentTypesWithContentIconAttribute :
                (_ContentTypesWithContentIconAttribute = GetPagesWithContentTypeIcons());
        }
    }
}
