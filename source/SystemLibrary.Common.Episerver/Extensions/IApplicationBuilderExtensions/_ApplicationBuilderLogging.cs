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
            appInError.Run(async context =>
            {
                if (context?.Response?.StatusCode != 503 && context?.Response?.StatusCode != 404)
                {
                    var contextFeature = context?.Features?.Get<IExceptionHandlerFeature>();
                    Log.Error(contextFeature?.Error);
                }
            });
        });
    }
}