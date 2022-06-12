using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Framework;
using EPiServer.ServiceLocation;

namespace SystemLibrary.Common.Episerver.Initialize
{
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class RemoveSuggestedContentTypes : InitModule
    {
        public override void ConfigureContainer(ServiceConfigurationContext context)
        {
            if (ServiceCollectionExtensions.Options.RemoveSuggestedContentTypes)
                Services.Remove<IContentTypeAdvisor>();
        }
    }
}