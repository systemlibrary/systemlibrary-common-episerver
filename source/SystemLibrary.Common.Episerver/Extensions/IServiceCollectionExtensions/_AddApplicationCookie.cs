using System;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void AddApplicationCookie(this IServiceCollection services, CommonCmsServicesOptions options)
    {
        if (!Options.AddApplicationCookie) return;
     
        services.ConfigureApplicationCookie(o =>
        {
            o.LoginPath = Options.CmsLoginPath ?? "/util/login";
            o.ExpireTimeSpan = TimeSpan.FromMinutes(Options.CmsUsersSignedInDurationMinutes);
            o.SlidingExpiration = Options.CmsUsersSlidingExpiration;
        });
    }
}
