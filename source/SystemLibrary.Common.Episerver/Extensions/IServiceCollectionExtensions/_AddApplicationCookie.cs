using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void AddApplicationCookie(this IServiceCollection services, CmsServicesCollectionOptions options)
    {
        if (!options.AddApplicationCookies) return;

        services.ConfigureApplicationCookie(opt =>
        {
            opt.ExpireTimeSpan = TimeSpan.FromMinutes(options.ApplicationCookieDuration);
            opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            opt.SlidingExpiration = options.UsersSlidingExpiration;
            opt.Cookie.HttpOnly = true;
        });

        services.AddAuthentication("Cookies")
        .AddCookie("Cookies", opt =>
        {
            opt.ExpireTimeSpan = TimeSpan.FromMinutes(options.CookieDuration);
            opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            opt.SlidingExpiration = options.UsersSlidingExpiration;
            opt.Cookie.HttpOnly = true;
        });

        if (options.CmsUsersMinimumPasswordLength > 3)
        {
            services.Configure<IdentityOptions>(o =>
            {
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(4);
                o.Lockout.MaxFailedAccessAttempts = 6;
                o.Lockout.AllowedForNewUsers = true;
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = options.CmsUsersMinimumPasswordLength;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequiredUniqueChars = 1;
            });
        }
    }
}
