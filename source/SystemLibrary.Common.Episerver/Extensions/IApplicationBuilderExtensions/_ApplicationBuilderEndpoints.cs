using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderEndpoints(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
                name: "SystemLibrary/Common/Episerver/UiHint/{controller}/{action}/{id?}",
                areaName: "SystemLibrary/Common/Episerver/UiHint/",
                pattern: "SystemLibrary/Common/Episerver/UiHint/{controller}/{action}/{id?}"
                );
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
                name: "SystemLibrary/Common/Episerver/{controller}/{action}/{id?}",
                areaName: "SystemLibrary/Common/Episerver/",
                pattern: "SystemLibrary/Common/Episerver/{controller}/{action}/{id?}"
                //was: pattern: "SystemLibrary/{controller=Home}/{action=Index}/{id?}"
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