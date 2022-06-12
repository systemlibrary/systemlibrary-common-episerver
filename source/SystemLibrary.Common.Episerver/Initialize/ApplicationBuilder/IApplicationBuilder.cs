using System.Net;

using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Rewrite;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Initialize
{
    /// <summary>
    /// Extension method on IApplicationBuilder to initialize Exception Handler, Static File Handler, Routing, Authorization and Authentication and more...
    /// </summary>
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

            //if (appBuilderOptions.UseExceptionHandler)
            //{
            //    app.UseExceptionHandler(appError =>
            //    {
            //        appError.Run(context =>
            //        {
            //            Dump.Write("ERRORO CCUUUUUUUUUUUURSED");
            //            try
            //            {
            //                var contextFeature = context?.Features?.Get<IExceptionHandlerFeature>();

            //                var logwriter = Services.Get<ILogWriter>();
            //                if(logwriter == null)
            //                {
            //                    Dump.Write(contextFeature?.Error);
            //                }
            //                else
            //                {
            //                    Log.Error(contextFeature?.Error);
            //                }
            //                if (context?.Response != null)
            //                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //            }
            //            catch
            //            {
            //                Dump.Write("Error occured " + context?.Request?.Path.ToString());
            //                Log.Error("Error occured " + context?.Request?.Path.ToString());
            //            }
                        

            //            return null;
            //        });
            //    });
            //}
            
            if (appBuilderOptions.UseRewriteEpiserverPathToEpiserverCms)
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
                     pattern: "SystemLibrary/{controller}/{action}/{id?}"
                    //was: pattern: "SystemLibrary/{controller=Home}/{action=Index}/{id?}"
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