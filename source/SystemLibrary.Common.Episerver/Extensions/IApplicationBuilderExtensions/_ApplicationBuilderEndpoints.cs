using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderEndpoints(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        app.UseEndpoints(endpoints =>
        {
            // /Cms folder in this project
            endpoints.MapAreaControllerRoute(
                name: Globals.AreaCms,
                areaName: Globals.AreaCms,
                pattern: Globals.AreaCms + "/{controller=Home}/{action=Index}/{id?}");

            //endpoints.MapAreaControllerRoute(
            //    name: Globals.AreaName,
            //    areaName: Globals.AreaName,
            //    pattern: Globals.AreaPath + "{area:exists}/{controller}/{action}/{id?}"
            //);

            // Note: Icons in page tree
            //endpoints.MapAreaControllerRoute(
            //   name: Globals.AreaName + "/{controller}/{action}/{id?}",
            //   areaName: Globals.AreaName,
            //   pattern: Globals.AreaName + "/{controller}/{action}/{id?}"
            //   );
        });

        if (options.MapEndpoints)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
            });
        }
    }
}