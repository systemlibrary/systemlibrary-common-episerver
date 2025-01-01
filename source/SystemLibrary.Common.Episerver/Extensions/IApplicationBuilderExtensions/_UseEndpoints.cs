using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void CmsPrecededEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapContent();
    }

    static void UseMapEndpoints(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
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
}