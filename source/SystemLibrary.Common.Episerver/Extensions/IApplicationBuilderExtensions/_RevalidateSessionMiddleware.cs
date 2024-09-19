using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver.Extensions;

internal class RevalidateSessionMiddleware
{
    RequestDelegate next;

    TimeSpan Threshold = TimeSpan.FromDays(30);


    public RevalidateSessionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context?.Request?.Path.Value?.ToLower();

        if (path?.StartsWith("/episerver") == true)
        {
            if (!path.EndsWith(".jpg") && 
                !path.EndsWith(".svg") &&
                !path.EndsWith(".css") &&
                !path.EndsWith(".js"))
            {
                var isNonWebSocketRequest = context?.WebSockets?.IsWebSocketRequest != true;

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
                                context.Response.Cookies.Delete(".AspNetCore.Identity.Application");
                                context.Response.Redirect("/");

                                Log.Error("Cookie session expired: " + Threshold + " days old, forcing a deletion and redirect to start");

                                return;
                            }
                        }
                    }
                }
            }
        }

        await next(context);
    }
}
