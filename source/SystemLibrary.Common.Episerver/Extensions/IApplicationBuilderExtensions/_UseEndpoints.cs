using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void UseMapEndpoints(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (options.MapContentEndpoints)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();

                // CMS folder in this project
                endpoints.MapAreaControllerRoute(
                    name: Globals.AreaCms,
                    areaName: Globals.AreaCms,
                    pattern: Globals.AreaCms + "/{controller=Home}/{action=Index}/{id?}");
            });
        }
        else
        {
            // CMS folder in this project
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: Globals.AreaCms,
                    areaName: Globals.AreaCms,
                    pattern: Globals.AreaCms + "/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}