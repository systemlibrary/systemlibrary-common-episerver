using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Framework;
using EPiServer.ServiceLocation;

namespace SystemLibrary.Common.Episerver.Initialize;

/// <summary>
/// An initialization module that removes 'Suggested Types' if configured in ServiceCollectionOptions, which you passed to CommonEpiserverServices()
/// </summary>
[InitializableModule]
[ModuleDependency(typeof(FrameworkInitialization))]
public class RemoveSuggestedContentTypes : InitModule
{
    public override void ConfigureContainer(ServiceConfigurationContext context)
    {
        if (ServiceCollectionExtensions.Options == null)
            Log.Error("Error: You've not called on extension for IServiceCollection named: CommonEpiserverServices()");

        if (ServiceCollectionExtensions.Options?.HideSuggestedContentTypes == true)
            Services.Remove<IContentTypeAdvisor>();
    }
}