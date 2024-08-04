using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void AddApplicationCookie(this IServiceCollection services, CmsServicesCollectionOptions options)
    {
        if (!Options.AddApplicationCookie) return;
     
        services.ConfigureApplicationCookie(o =>
        {
            o.ExpireTimeSpan = TimeSpan.FromMinutes(Options.CmsUsersSignedInDurationMinutes);
            o.SlidingExpiration = Options.CmsUsersSlidingExpiration;
        });
    }
}
