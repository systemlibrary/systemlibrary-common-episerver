using System;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void ServiceCollectionCookies(IServiceCollection services, CommonEpiserverApplicationServicesOptions options)
    {
        if (!Options.ConfigureApplicationCookie) return;
     
        services.ConfigureApplicationCookie(o =>
        {
            o.LoginPath = Options.CmsLoginPath ?? "/util/login";
            o.ExpireTimeSpan = TimeSpan.FromMinutes(Options.CmsUserSessionDurationMinutes);
            o.SlidingExpiration = Options.CmsUsersSlidingExpiration;
        });
    }
}
