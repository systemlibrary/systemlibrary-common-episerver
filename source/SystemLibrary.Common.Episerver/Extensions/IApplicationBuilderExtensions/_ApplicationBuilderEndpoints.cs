using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderEndpoints(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        app.UseEndpoints(endpoints =>
        {
            // Note: /Cms/ refers to the folder named 'Cms' at root of the library and all its inner controllers
            endpoints.MapAreaControllerRoute(
                name: Globals.AreaName + "Cms",
                areaName: Globals.AreaName + "Cms/",
                pattern: Globals.AreaName + "Cms/{controller}/{action}/{id?}"
            );
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
                name: Globals.AreaName + "{controller}/{action}/{id?}",
                areaName: Globals.AreaName,
                pattern: Globals.AreaName + "{controller}/{action}/{id?}"
                //was: pattern: "SystemLibrary/{controller=Home}/{action=Index}/{id?}"
                );
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
                name: "/imageshopextended/{controller}/{action}",
                areaName: "imageshopextended/",
                pattern: "imageshopextended/" + "{controller}/{action}/{id?}"
                );
        });

        if (options.UseEpiserverMapContentEndpoints)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
            });
        }
    }
}