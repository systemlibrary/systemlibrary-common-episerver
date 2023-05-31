using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderLogging(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        if (!options.UseExceptionLogging) return;

        app.UseExceptionHandler(appError =>
        {
            appError.Run(context =>
            {
                if(context?.Response?.StatusCode == 503)
                    return System.Threading.Tasks.Task.FromResult(0);

                var contextFeature = context?.Features?.Get<IExceptionHandlerFeature>();

                if (context?.Response != null)
                {
                    if (context.Response.StatusCode < 300)
                        context.Response.StatusCode = 500;
                }

                Log.Error(contextFeature?.Error);

                return System.Threading.Tasks.Task.FromResult(0);
            });
        });
    }
}