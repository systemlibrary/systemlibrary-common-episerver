using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IServiceCollectionExtensions
{
    static void AddApplicationCookie(this IServiceCollection services, CmsFrameworkOptions options)
    {
        services.ConfigureApplicationCookie(opt =>
        {
            opt.Cookie.SameSite = SameSiteMode.Strict;
            opt.Cookie.HttpOnly = true;
            opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            opt.SlidingExpiration = options.ApplicationCookieSlidingExpiration;
            opt.ExpireTimeSpan = TimeSpan.FromMinutes(options.ApplicationCookieDuration);

            opt.LoginPath = "/util/login/";
        });

        if (options.ApplicationCookieMimumPasswordLength > 0)
        {
            services.Configure<IdentityOptions>(o =>
            {
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                o.Lockout.MaxFailedAccessAttempts = 10;
                o.Lockout.AllowedForNewUsers = true;
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = options.ApplicationCookieMimumPasswordLength;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequiredUniqueChars = 0;
            });
        }
    }
}
