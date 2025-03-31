using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    // Any library errors serving library static files, we ignore to prevent dojo from crashing
    static void SkipStaticLibraryRequestErrors(this IApplicationBuilder app, CmsFrameworkOptions options)
    {
        app.Use(async (context, next) =>
        {
            var path = context?.Request?.Path.Value;

            if (path != null)
            {
                var l = path.Length;
                if (path[l - 1] != '/')
                {
                    if (l > 34 && context.Response.StatusCode == 404 && path.StartsWith("/SystemLibrary/CommonEpiserverCms", StringComparison.Ordinal))
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("");
                        // NOTE: No return statement, as we continue if theres anything "below"
                    }
                }
            }

            await next();
        });
    }
}