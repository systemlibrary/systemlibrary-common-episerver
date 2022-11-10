﻿using EPiServer.Web.Routing;

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
                name: "SystemLibrary/Common/Episerver/Cms/{controller}/{action}/{id?}",
                areaName: "SystemLibrary/Common/Episerver/Cms/",
                pattern: "SystemLibrary/Common/Episerver/Cms/{controller}/{action}/{id?}"
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