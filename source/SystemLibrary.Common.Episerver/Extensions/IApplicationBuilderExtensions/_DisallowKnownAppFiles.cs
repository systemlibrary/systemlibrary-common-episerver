using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void DisallowKnownAppFiles(this IApplicationBuilder app, CmsFrameworkOptions options)
    {
        if (!options.DisallowKnownAppFiles) return;

        app.Use(async (context, next) =>
        {
            var path = context?.Request?.Path.Value;

            if (path != null)
            {
                var l = path.Length;
                if (l > 4 && path[l - 1] != '/')
                {
                    if (!(path.EndsWith(".js", StringComparison.Ordinal) ||
                        path.EndsWith(".png", StringComparison.Ordinal) ||
                        path.EndsWith(".jpg", StringComparison.Ordinal) ||
                        path.EndsWith(".css", StringComparison.Ordinal) ||
                        path.EndsWith(".html", StringComparison.Ordinal) ||
                        path.EndsWith(".gif", StringComparison.Ordinal) ||
                        path.EndsWith(".webp", StringComparison.Ordinal) ||
                        path.EndsWith(".json", StringComparison.Ordinal) ||
                        path.EndsWith(".svg", StringComparison.Ordinal) ||
                        path.EndsWith(".txt", StringComparison.Ordinal) ||
                        path.EndsWith(".pdf", StringComparison.Ordinal)))
                    {
                        if (path.EndsWith(".dll", StringComparison.Ordinal) ||
                            path.EndsWith(".cs", StringComparison.Ordinal) ||
                            path.EndsWith(".ts", StringComparison.Ordinal) ||
                            path.EndsWith(".tsx", StringComparison.Ordinal) ||
                            path.EndsWith(".mdf", StringComparison.Ordinal) ||
                            path.EndsWith(".sql", StringComparison.Ordinal) ||
                            path.EndsWith(".db", StringComparison.Ordinal) ||
                            path.EndsWith(".cshtml", StringComparison.Ordinal) ||
                            path.EndsWith(".config", StringComparison.Ordinal) ||
                            path.EndsWith(".DLL", StringComparison.Ordinal) ||
                            path.EndsWith(".CS", StringComparison.Ordinal) ||
                            path.EndsWith(".CSHTML", StringComparison.Ordinal) ||
                            path.EndsWith(".CONFIG", StringComparison.Ordinal) ||
                            path.EndsWith(".Dll", StringComparison.Ordinal) ||
                            path.EndsWith(".Cs", StringComparison.Ordinal) ||
                            path.EndsWith(".Config", StringComparison.Ordinal) ||
                            path.EndsWith(".Cshtml", StringComparison.Ordinal))
                        {
                            Log.Debug("DisallowKnownAppFiles is True, and path request is a Known App File extension: " + path);
                            return;
                        }

                        if (l > 11)
                        {
                            if (path[1] == 'a' || path[1] == 'A' ||
                                path[1] == 'c' || path[1] == 'C')
                            {
                                var p = path.ToLower();
                                if (p.StartsWith("/appsettings.", StringComparison.Ordinal) ||
                                        p.StartsWith("/configurations/", StringComparison.Ordinal) ||
                                        p.StartsWith("/configuration/", StringComparison.Ordinal) ||
                                        p.StartsWith("/config/", StringComparison.Ordinal) ||
                                        p.StartsWith("/configs/", StringComparison.Ordinal))
                                {
                                    Log.Debug("DisallowKnownAppFiles is True, and path request is a Known App File Config: " + path);
                                    return;
                                }
                            }
                        }

                        if (l > 8)
                        {
                            if (path[1] == 'b' || path[1] == 'B')
                            {
                                var p = path.ToLower();
                                if (p.StartsWith("/bin/", StringComparison.Ordinal))
                                {
                                    Log.Debug("DisallowKnownAppFiles is True, and path request is a Known App Path: " + path);
                                    return;
                                }
                            }
                        }
                    }

                    if (l > 34 && context.Response.StatusCode == 404 && path.StartsWith("/SystemLibrary/CommonEpiserverCms", StringComparison.Ordinal))
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("");
                        // TODO: careful, this has no return statement, why?
                        // We cant to continue pipeline if theres anything "below"?
                        // return;
                    }
                }
            }

            await next();
        });
    }
}