using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Extensions;

internal class IdentityCookieRevalidationMiddleware
{
    RequestDelegate next;

    TimeSpan Threshold = TimeSpan.FromDays(30);

    static string CmsUserCookieName;

    static IdentityCookieRevalidationMiddleware()
    {
        var cookieOptions = Services.Get<IOptionsMonitor<CookieAuthenticationOptions>>();

        CmsUserCookieName = cookieOptions?.Get("Identity.Application")?.Cookie?.Name;
    }


    public IdentityCookieRevalidationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (CmsUserCookieName != null)
        {
            var path = context?.Request?.Path.Value?.ToLower();

            if (path?.StartsWith("/episerver") == true)
            {
                if (!path.EndsWith(".jpg") &&
                    !path.EndsWith(".svg") &&
                    !path.EndsWith(".css") &&
                    !path.EndsWith(".png") &&
                    !path.EndsWith(".gif") &&
                    !path.EndsWith(".js"))
                {
                    var isNonWebSocketRequest = context.WebSockets?.IsWebSocketRequest != true;

                    if (isNonWebSocketRequest)
                    {
                        var authTicket = await context.AuthenticateAsync("Identity.Application");

                        if (authTicket?.Succeeded == true)
                        {
                            var userPrincipal = authTicket.Principal;

                            var issuedUtc = authTicket.Properties.IssuedUtc;
                            if (issuedUtc.HasValue)
                            {
                                var cookieAge = DateTimeOffset.UtcNow - issuedUtc.Value;

                                if (cookieAge > Threshold)
                                {
                                    context.Response.Cookies.Delete(CmsUserCookieName);
                                    context.Response.Redirect("/");

                                    Log.Error("[IdentityCookieRevalidationMiddleware] Cookie session expired: " + Threshold + " days old, forcing a deletion and redirect to start");

                                    return;
                                }
                            }
                            else
                            {
                                Log.Debug("[IdentityCookieRevalidationMiddleware] Issued date was not set on the cookie, doing nothing...");
                            }
                        }
                    }
                }
            }
        }

        await next(context);
    }
}
