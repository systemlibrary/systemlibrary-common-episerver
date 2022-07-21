using System;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class ServiceCollectionExtensions
{
    static void ServiceCollectionCookies(IServiceCollection services, CommonEpiserverApplicationServicesOptions options)
    {
        if (!Options.ConfigureApplicationCookie) return;
     
        services.ConfigureApplicationCookie(o =>
        {
            o.LoginPath = "/util/login";
            o.ExpireTimeSpan = TimeSpan.FromMinutes(Options.CmsUserSessionDurationMinutes);
            o.SlidingExpiration = Options.CmsUsersSlidingExpiration;
        });
    }
}
