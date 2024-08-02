using System.Reflection;

using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

using SystemLibrary.Common.Episerver.FontAwesome;
using SystemLibrary.Common.Episerver.Initialize;
using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Attributes;

partial class ContentIconAttribute
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    internal partial class ContentIconEditorDescriptorModule : StartupModule
    {
        //Item1: Type is "StartPage"
        //Item2: ContentIcon is the attribute "on type"
        //Item3: string 1 is the css class for pagetree icons
        //Item4: string 2 is the custom relative icon url, as in: not using a FontAwesome Icon
        static internal List<Tuple<Type, ContentIconAttribute, string, string>> ContentDescriptorSettings = new List<Tuple<Type, ContentIconAttribute, string, string>>();

        static bool IsInitialized;

        static ContentIconEditorDescriptorModule()
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

        public override void Initialize(InitializationEngine context)
        {
            if (IsInitialized || context.HostType != HostType.WebApplication) return;

            IsInitialized = true;

            try
            {
                //Note: on a rare occasion this has thrown an exception in a production environment, try catching it
                //- swallow error, as we do not want app to crash for this
                var descriptorRegistry = context.Locate?.Advanced?.GetInstance<UIDescriptorRegistry>();
                
                if (descriptorRegistry?.UIDescriptors == null) return;

                if (ContentDescriptorSettings.Count == 0) return;

                SetUiDescriptors(descriptorRegistry);
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                Log.Error(ex);
            }
        }

        static void SetUiDescriptors(UIDescriptorRegistry descriptorRegistry)
        {
            foreach (var descriptor in descriptorRegistry.UIDescriptors)
            {
                if (descriptor == null) continue;

                foreach (var contentType in ContentDescriptorSettings)
                {
                    if (contentType.Item1 == descriptor.ForType)
                    {
                        SetDescriptorClassForFontAwesomeIcons(descriptor, contentType);

                        SetDescriptorClassForContentTypesWithRelativeIconUrl(descriptor, contentType);
                    }
                }
            }
        }

        static void SetDescriptorClassForContentTypesWithRelativeIconUrl(UIDescriptor descriptor, Tuple<Type, ContentIconAttribute, string, string> contentType)
        {
            if (contentType.Item1.FullName == descriptor.ForType.FullName && contentType.Item4.Is())
                descriptor.IconClass += " epi-iconObjectPage " + contentType.Item3;
        }

        static void SetDescriptorClassForFontAwesomeIcons(UIDescriptor descriptor, Tuple<Type, ContentIconAttribute, string, string> contentType)
        {
            if (contentType.Item2.Value == null || contentType.Item4.Is()) return;
            
            var fontAwesomeIconName = contentType.Item2.Value.GetType();

            var iconName = contentType.Item2.Value.ToString();
            if (iconName.StartsWith("__enum_"))
                iconName = iconName.Substring(3);

            var fontawesomeIcon = iconName.Replace("_", "-");

            descriptor.IconClass = "dijitIcon dijitTreeIcon epi-iconObjectPage ";

            if (fontAwesomeIconName == typeof(FontAwesomeSolid))
            {
                descriptor.IconClass += "fas fa-" + fontawesomeIcon;
            }
            else if (fontAwesomeIconName == typeof(FontAwesomeBrands))
            {
                descriptor.IconClass += "fab fa-" + fontawesomeIcon;
            }
            else
            {
                descriptor.IconClass += "far fa-" + fontawesomeIcon;
            }

            var isFolder = contentType.Item2.IsFolder();

            if (isFolder)
            {
                descriptor.IconClass += " custom-common-episerver-page-tree-icon__folder";
            }
            else
            {
                var isSetting = contentType.Item2.IsSettings();
                if(isSetting)
                    descriptor.IconClass += " custom-common-episerver-page-tree-icon__setting";
            }
        }
    }
}
