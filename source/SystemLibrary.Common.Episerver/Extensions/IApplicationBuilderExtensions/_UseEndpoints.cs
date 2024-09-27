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

                endpoints.MapControllers();

                // NOTE: This actually matches the route of a C# BlockController before the "Url" of a Page
                // if the path matches, so for instance "/ButtonBlock" triggers directly the ButtonBlockController, even
                // if a page has been created with /ButtonBlock as a path
                // endpoints.MapDefaultControllerRoute();

                if (options.UseRazorPages)
                    endpoints.MapRazorPages();

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
            if (options.UseControllers)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapDefaultControllerRoute();

                    if (options.UseRazorPages)
                        endpoints.MapRazorPages();

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
            else
            {
                if (options.UseRazorPages)
                {
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();

                        // CMS folder in this project
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
    }
}