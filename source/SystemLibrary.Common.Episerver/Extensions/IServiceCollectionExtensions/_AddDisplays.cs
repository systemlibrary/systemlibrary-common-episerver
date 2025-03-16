using EPiServer.ServiceLocation;
using EPiServer.Web;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SystemLibrary.Common.Episerver.Display;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void AddDisplays(this IServiceCollection services, CmsFrameworkOptions options)
    {
        services.Configure<DisplayOptions>(displayOption =>
        {
            displayOption.Add("desktop", "Desktop", "col-12", "", "epi-icon__layout--full");
            displayOption.Add("tablet", "Tablet", "col-8", "", "epi-icon__layout--two-thirds");
            displayOption.Add("mobile", "Mobile", "col-4", "", "epi-icon__layout--one-third");
        });

        services.TryAddSingleton<DesktopResolution>();
        services.TryAddSingleton<TabletResolution>();
        services.TryAddSingleton<MobileResolution>();
    }
}
