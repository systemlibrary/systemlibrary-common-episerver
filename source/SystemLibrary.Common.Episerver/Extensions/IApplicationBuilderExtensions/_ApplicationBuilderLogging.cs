using System;
using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderLogging(IApplicationBuilder app, EpiserverAppBuilderOptions options)
    {
        if (!options.UseExceptionLogging) return;

        app.UseExceptionHandler(appError =>
        {
            appError.Run(context =>
            {
                try
                {
                    var contextFeature = context?.Features?.Get<IExceptionHandlerFeature>();

                    var logwriter = Services.Get<ILogWriter>();
                    if (logwriter == null)
                    {
                        Dump.Write(contextFeature?.Error);
                    }
                    else
                    {
                        try
                        {
                            Log.Error(contextFeature?.Error);
                        }
                        catch
                        {
                            Dump.Write(contextFeature?.Error);
                        }
                    }

                    if (!options.UseExceptionPageInTestAndDev)
                    {
                        if (context?.Response != null)
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
                catch (Exception ex)
                {
                    Dump.Write("Error occured inside the appErrorHandler, on path: " + context?.Request?.Path.ToString() + ": " + ex.Message);
                }
                return null;
            });
        });
    }
}