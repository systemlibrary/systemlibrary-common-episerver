using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Framework;
using EPiServer.ServiceLocation;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Initialize;

/// <summary>
/// An initialization module that removes 'Suggested Types' if configured in ServiceCollectionOptions, which you passed to CommonEpiserverServices()
/// </summary>
[InitializableModule]
[ModuleDependency(typeof(FrameworkInitialization))]
public class RemoveSuggestedContentTypes : StartupModule
{
    public override void ConfigureContainer(ServiceConfigurationContext context)
    {
        if (Extensions.IServiceCollectionExtensions.Options == null)
            Log.Error("Error: You've not called on extension for IServiceCollection named: CommonEpiserverServices()");

        if (Extensions.IServiceCollectionExtensions.Options?.HideSuggestedContentTypes == true)
            Services.Remove<IContentTypeAdvisor>();
    }
}