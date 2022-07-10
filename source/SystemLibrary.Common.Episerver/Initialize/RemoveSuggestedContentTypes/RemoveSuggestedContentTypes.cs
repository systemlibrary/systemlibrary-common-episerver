using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Framework;
using EPiServer.ServiceLocation;

namespace SystemLibrary.Common.Episerver.Initialize;


[InitializableModule]
[ModuleDependency(typeof(FrameworkInitialization))]
public class RemoveSuggestedContentTypes : InitModule
{
    public override void ConfigureContainer(ServiceConfigurationContext context)
    {
        if (ServiceCollectionExtensions.Options == null)
            Log.Error("Error: You've not called on extension for IServiceCollection named: CommonEpiserverServices()");

        if (ServiceCollectionExtensions.Options?.RemoveSuggestedContentTypes == true)
            Services.Remove<IContentTypeAdvisor>();
    }
}