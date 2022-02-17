using System.Net;

using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Rewrite;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Initialize
{
    public static class IApplicationBuilder
    {
        /// <summary>
        /// Registers:
        /// ExceptionHandler, ForwardProxyHeader, Static File Handler, Routing Handler, Authorization, Authentication and Redirect /episerver to /episerver/cms
        /// </summary>
        public static Microsoft.AspNetCore.Builder.IApplicationBuilder CommonEpiserverAppBuilder(this Microsoft.AspNetCore.Builder.IApplicationBuilder app, ApplicationBuilderOptions appBuilderOptions = null)
        {
            if(appBuilderOptions == null)
                appBuilderOptions = new ApplicationBuilderOptions();

            if (appBuilderOptions.UseExceptionHandler)
            {
                app.UseExceptionHandler(appError =>
                {
                    appError.Run(context =>
                    {
                        try
                        {
                            var contextFeature = context?.Features?.Get<IExceptionHandlerFeature>();

                            Log.Error(contextFeature?.Error);

                            if (context?.Response != null)
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }
                        catch
                        {
                            Log.Error("Error occured " + context?.Request?.Path.ToString());
                        }

                        return null;
                    });
                });
            }
            
            if (appBuilderOptions.UseEpiserverRewriteToEpiserverCms)
            {
                var options = new RewriteOptions()
                .AddRedirect("episerver$", "episerver/cms/")
                .AddRedirect("EPiServer$", "episerver/cms/")
                .AddRedirect("episerver/$", "episerver/cms/");

                app.UseRewriter(options);
            }

            app.CommonAppBuilder(appBuilderOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("SystemLibrary",
                     "SystemLibrary",
                     pattern: "SystemLibrary/{controller=Home}/{action=Index}/{id?}"
                    );
            });

            if (appBuilderOptions.UseEpiserverEndspoints)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapContent();
                });
            }

            return app;
        }
    }
}