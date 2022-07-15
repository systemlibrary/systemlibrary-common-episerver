using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class ServiceCollectionExtensions
{
    static void ServiceCollectionCaching(IServiceCollection services, EpiserverServiceCollectionOptions options)
    {
        services.AddResponseCaching();
    }
}
