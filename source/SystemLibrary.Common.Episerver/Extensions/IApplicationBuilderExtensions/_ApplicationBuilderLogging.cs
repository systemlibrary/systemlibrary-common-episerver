using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderLogging(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        if (!options.UseExceptionLogging) return;

        app.UseExceptionHandler(appInError =>
        {
            Dump.Write("APP IN ERROR?1");
            appInError.Run(async context =>
            {
                Dump.Write("APP IN ERROR?");
                var contextFeature = context?.Features?.Get<IExceptionHandlerFeature>();

                if (context?.Response != null)
                {
                    if (!context.Response.HasStarted)
                        if (context.Response.StatusCode < 300)
                            context.Response.StatusCode = 500;
                }

                if(context?.Response?.StatusCode != 503)
                    Log.Error(contextFeature?.Error);
            });
        });
    }
}