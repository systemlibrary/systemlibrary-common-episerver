using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Initialize;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderEndpoints(IApplicationBuilder app, IApplicationBuilderOptions options)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute("SystemLibrary",
                 "SystemLibrary",
                 pattern: "SystemLibrary/{controller}/{action}/{id?}"
                //was: pattern: "SystemLibrary/{controller=Home}/{action=Index}/{id?}"
                );
        });

        if (!options.UseEpiserverEndpoints) return;

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
        });
    }
}