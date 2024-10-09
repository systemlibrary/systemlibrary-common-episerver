using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ExceptionHandler(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (!options.UseExceptionHandler) return;

        app.UseExceptionHandler(appInError =>
        {
            appInError.Run(context =>
            {
                var statusCode = context?.Response?.StatusCode ?? 0;
                if (statusCode > 399 && statusCode != 404 && statusCode != 503)
                {
                    var contextFeature = context?.Features?.Get<IExceptionHandlerFeature>();

                    if (contextFeature?.Error != null)
                        Log.Error(contextFeature?.Error);
                }
                return null;
            });
        });
    }
}