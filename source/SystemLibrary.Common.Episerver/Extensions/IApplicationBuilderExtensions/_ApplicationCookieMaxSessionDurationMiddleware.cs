﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Episerver.Extensions;

internal class ApplicationCookieMaxSessionDurationMiddleware
{
    RequestDelegate next;

    TimeSpan Threshold = TimeSpan.FromDays(30);

    static string CmsUserCookieName;

    static ApplicationCookieMaxSessionDurationMiddleware()
    {
        var cookieOptions = Services.Get<IOptionsMonitor<CookieAuthenticationOptions>>();

        CmsUserCookieName = cookieOptions?.Get("Identity.Application")?.Cookie?.Name;
    }

    public ApplicationCookieMaxSessionDurationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (CmsUserCookieName != null)
        {
            var path = context?.Request?.Path.Value;

            if (path?.StartsWith("/EPiServer/CMS", StringComparison.OrdinalIgnoreCase) == true ||
                path?.StartsWith("/EPiServer/EPiServer.Cms.UI.Admin", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (!path.IsFile())
                {
                    var isNonWebSocketRequest = context.WebSockets?.IsWebSocketRequest != true;

                    if (isNonWebSocketRequest)
                    {
                        try
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
                                        var currentUser = new AppCurrentUser();

                                        Log.Warning("[ApplicationCookieMaxSessionDurationMiddleware] Cookie expired by threshold (timespan): " + Threshold + ". Deleting cookie and redirect to start for user: " + currentUser.Id + " " + currentUser.GivenName);

                                        context.Response.Cookies.Delete(CmsUserCookieName);
                                        context.Response.Redirect("/");

                                        return;
                                    }
                                }
                                else
                                {
                                    Log.Debug("[ApplicationCookieMaxSessionDurationMiddleware] Issued date was not set on the cookie, doing nothing...");
                                }
                            }
                        }
                        catch
                        {
                            // Swallow: if the Identity.Application throws it most likely is not registered, so we just ignore it
                        }
                    }
                }
            }
        }

        await next(context);
    }
}
