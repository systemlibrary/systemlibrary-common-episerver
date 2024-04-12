using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void UseMapEndpoints(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (options.UseMapContentAndControllers)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();

                endpoints.MapDefaultControllerRoute();

                if (options.UseApiControllers)
                    endpoints.MapControllerRoute("api/{controller}/{action}/{id?}", "api/{controller}/{action}/{id?}");

                endpoints.MapAreaControllerRoute(
                   name: Globals.AreaCms,
                   areaName: Globals.AreaCms,
                   pattern: Globals.AreaCms + "/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: Globals.AreaFontAwesome,
                    areaName: Globals.AreaFontAwesome,
                    pattern: Globals.AreaFontAwesome + "/{controller=Home}/{action=Index}/{id?}");
            });
        }
        else
        {
            // CMS folder in this project
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: Globals.AreaFontAwesome,
                    areaName: Globals.AreaFontAwesome,
                    pattern: Globals.AreaFontAwesome + "/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: Globals.AreaCms,
                    areaName: Globals.AreaCms,
                    pattern: Globals.AreaCms + "/{controller=Home}/{action=Index}/{id?}");
            });
        }


    }
}