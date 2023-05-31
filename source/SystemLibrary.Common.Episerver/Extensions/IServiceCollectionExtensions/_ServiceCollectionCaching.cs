using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void ServiceCollectionCaching(IServiceCollection services, CommonEpiserverApplicationServicesOptions options)
    {
        services.AddResponseCaching();
    }
}
