using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Initialize;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderEndpoints(IApplicationBuilder app, IApplicationBuilderOptions options)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
                name: "SystemLibrary/Common/Episerver/{controller}/{action}/{id?}",
                areaName: "SystemLibrary",
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