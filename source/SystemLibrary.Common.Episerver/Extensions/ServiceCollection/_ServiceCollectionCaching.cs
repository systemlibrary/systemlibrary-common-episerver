using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class ServiceCollectionExtensions
{
    static void ServiceCollectionCaching(IServiceCollection services, ServiceCollectionEpiserverOptions options)
    {
        services.AddResponseCaching();
    }
}
