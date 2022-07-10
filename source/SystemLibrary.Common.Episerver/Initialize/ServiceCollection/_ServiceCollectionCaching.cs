using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Initialize;

partial class ServiceCollectionExtensions
{
    static void ServiceCollectionCaching(IServiceCollection services, ServiceCollectionOptions options)
    {
        services.AddResponseCaching();
    }
}
