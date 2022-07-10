using System;

using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

using SystemLibrary.Common.Episerver.Initialize;

namespace SystemLibrary.Common.Episerver.Attributes;

partial class ContentIconAttribute
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Cms.Shell.InitializableModule))]
    internal partial class ContentIconRouteModule : InitModule
    {
        static bool IsInitialized;

        public override void Initialize(InitializationEngine context)
        {
            if (IsInitialized || context.HostType != HostType.WebApplication) return;

            IsInitialized = true;

            var uiDescriptorRegistry = context.Locate?.Advanced?.GetInstance<UIDescriptorRegistry>();

            try
            {
                //NOTE: On rare occasions this throws
                //- side effect that icons will not be loaded
                //- swallow error, as we do not want app to crash for this
                if (uiDescriptorRegistry?.UIDescriptors == null) return;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return;
            }

            SetUIDescriptorCssClass(uiDescriptorRegistry);
        }

        static void SetUIDescriptorCssClass(UIDescriptorRegistry uiDescriptorRegistry)
        {
            foreach (var uiDescriptor in uiDescriptorRegistry.UIDescriptors)
            {
                if (uiDescriptor == null || uiDescriptor.ForType == null) continue;

                var contentIconAttribute = GetContentIconAttribute(uiDescriptor.ForType.Name);

                if (contentIconAttribute == null || contentIconAttribute.Value == null) continue;

                var fontAwesomeIconName = contentIconAttribute.Value.GetType();

                var iconName = contentIconAttribute.Value.ToString();
                if (iconName.StartsWith("___"))
                    iconName = iconName.Substring(3);

                var fontawesomeIcon = iconName.Replace("_", "-");

                uiDescriptor.IconClass = "dijitIcon dijitTreeIcon epi-iconObjectPage ";

                if (fontAwesomeIconName == typeof(FontAwesomeSolid))
                {
                    uiDescriptor.IconClass += "fas fa-" + fontawesomeIcon;
                }
                else if (fontAwesomeIconName == typeof(FontAwesomeBrands))
                {
                    uiDescriptor.IconClass += "fab fa-" + fontawesomeIcon;
                }
                else
                {
                    uiDescriptor.IconClass += "far fa-" + fontawesomeIcon;
                }
            }
        }
    }
}
