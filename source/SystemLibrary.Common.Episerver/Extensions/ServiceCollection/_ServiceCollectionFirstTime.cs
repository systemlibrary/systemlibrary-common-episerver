using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SystemLibrary.Common.Episerver.Initialize;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class ServiceCollectionExtensions
{
    static void ServiceCollectionFirstTime(IServiceCollection services, EpiserverServiceCollectionOptions options)
    {
        services.TryAddSingleton<ServiceAccessor<IContentRouteHelper>>(locator => locator.GetInstance<IContentRouteHelper>);

        services.Add(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton(typeof(IFirstRequestInitializer), typeof(WebApplicationInitializer)));
    }
}
