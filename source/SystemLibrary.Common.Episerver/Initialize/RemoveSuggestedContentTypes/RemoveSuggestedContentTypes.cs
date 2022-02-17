using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace SystemLibrary.Common.Episerver.Initialize
{
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class RemoveSuggestedContentTypes : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            if (ServiceCollectionExtensions.Options.RemoveSuggestedContentTypes)
                Services.Remove<IContentTypeAdvisor>();
        }

        public void Initialize(InitializationEngine context)
        {
        }
        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}