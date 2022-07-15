using EPiServer.ServiceLocation;
using EPiServer.Web;

using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Episerver.Display;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class ServiceCollectionExtensions
{
    static void ServiceCollectionDisplays(IServiceCollection services, EpiserverServiceCollectionOptions options)
    {
        if (!Options.RegisterDisplays) return;

        services.Configure<DisplayOptions>(displayOption =>
        {
            displayOption.Add("desktop", "Desktop", "col-12", "", "epi-icon__layout--full");
            displayOption.Add("tablet", "Tablet", "col-8", "", "epi-icon__layout--two-thirds");
            displayOption.Add("mobile", "Mobile", "col-4", "", "epi-icon__layout--one-third");
        });

        services.AddSingleton<DesktopResolution>();
        services.AddSingleton<TabletResolution>();
        services.AddSingleton<MobileResolution>();
    }
}
