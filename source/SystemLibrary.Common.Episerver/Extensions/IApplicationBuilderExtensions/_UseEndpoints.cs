using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void UseEndpoints(this IApplicationBuilder app, UseCommonEpiserverAppOptions options)
    {
        app.UseEndpoints(endpoints =>
        {
            // CMS folder in this project
            endpoints.MapAreaControllerRoute(
                name: Globals.AreaCms,
                areaName: Globals.AreaCms,
                pattern: Globals.AreaCms + "/{controller=Home}/{action=Index}/{id?}");
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