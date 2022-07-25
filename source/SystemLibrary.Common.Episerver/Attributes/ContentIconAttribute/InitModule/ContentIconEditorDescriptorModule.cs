using System;

using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

using SystemLibrary.Common.Episerver.FontAwesome;
using SystemLibrary.Common.Episerver.Initialize;

namespace SystemLibrary.Common.Episerver.Attributes;

partial class ContentIconAttribute
{
    [InitializableModule]
    internal partial class ContentIconEditorDescriptorModule : StartupModule
    {
        static bool IsInitialized;

        public override void Initialize(InitializationEngine context)
        {
            if (IsInitialized || context.HostType != HostType.WebApplication) return;

            IsInitialized = true;

            var descriptorRegistry = context.Locate?.Advanced?.GetInstance<UIDescriptorRegistry>();

            try
            {
                //NOTE: On rare occasions this throws
                //- side effect that icons will not be loaded
                //- swallow error, as we do not want app to crash for this
                if (descriptorRegistry?.UIDescriptors == null) return;
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                Log.Error(ex);
                return;
            }

            LoadPageTypesWithIconAttribute();

            if (ContentDescriptorSettings.Count == 0) return;

            SetUiDescriptors(descriptorRegistry);
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
