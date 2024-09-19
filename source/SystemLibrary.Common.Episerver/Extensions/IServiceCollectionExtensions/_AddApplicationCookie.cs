using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void AddApplicationCookie(this IServiceCollection services, CmsServicesCollectionOptions options)
    {
        if (!options.AddApplicationCookie) return;
     
        services.ConfigureApplicationCookie(o =>
        {
            o.ExpireTimeSpan = TimeSpan.FromMinutes(options.CmsUsersSignedInDurationMinutes);
            o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            o.Cookie.HttpOnly = true;
            o.SlidingExpiration = options.CmsUsersSlidingExpiration;
        });

        if (options.CmsUsersMinimumPasswordLength > 0)
        {
            services.Configure<IdentityOptions>(o =>
            {
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(12);
                o.Lockout.MaxFailedAccessAttempts = 6;
                o.Lockout.AllowedForNewUsers = true;
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = options.CmsUsersMinimumPasswordLength;
                o.Password.RequireNonAlphanumeric = false;
            });
        }
    }
}
