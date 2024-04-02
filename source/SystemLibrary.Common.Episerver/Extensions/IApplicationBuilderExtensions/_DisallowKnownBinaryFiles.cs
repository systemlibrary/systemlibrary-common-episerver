using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void DisallowKnownAppFiles(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (!options.DisallowKnownAppFiles) return;

        app.Use(async (context, next) =>
        {
            var path = context?.Request?.Path.Value;

            if (path != null)
            {
                var l = path.Length;
                if (l > 4 && path[l - 1] != '/' && !path.EndsWith(".js") && !path.EndsWith(".png") && !path.EndsWith(".jpg") && !path.EndsWith(".css"))
                {
                    try
                    {
                        if (path.EndsWithAnyCaseInsensitive(".dll", ".cs", ".cshtml", ".config"))
                        {
                            return;
                        }

                        if (l > 13)
                        {
                            if (path[1] == 'a' || path[1] == 'A' ||
                                path[1] == 'c' || path[1] == 'C')
                            {
                                var p = path.ToLower();
                                if (p.StartsWith("/appsettings.") ||
                                        p.StartsWith("/configurations/") ||
                                        p.StartsWith("/configuration/") ||
                                        p.StartsWith("/config/") ||
                                        p.StartsWith("/configs/"))
                                {
                                    return;
                                }
                            }
                        }

                        if (l > 7)
                        {
                            if (path[1] == 'b' || path[1] == 'B')
                            {
                                var p = path.ToLower();
                                if (p.StartsWith("/bin/"))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                if (context.Response.StatusCode == 404 && path.StartsWith("/SystemLibrary/CommonEpiserverCms"))
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("");
                }
            }

            await next();
        });
    }
}