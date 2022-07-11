using System;

using Microsoft.AspNetCore.Builder;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Initialize;

public static partial class IApplicationBuilderExtensions
{
    public static IApplicationBuilder CommonEpiserverAppBuilder(this IApplicationBuilder app, IApplicationBuilderOptions options = null)
    {
        if(!System.IO.File.Exists("module.config"))
        {
            throw new Exception("Module.config is not located at root, cannot continue with CommonEpiServer initialization");
        }
        if (options == null)
            options = new IApplicationBuilderOptions();

        ApplicationBuilderLogging(app, options);

        ApplicationBuilderCompression(app, options);

        //MapContent() from Episerver calls UseRazorPages()
        options.UseRazorPagesEndpoints = false;

        app.CommonWebApplicationBuilder(options);

        ApplicationBuilderEndpoints(app, options);

        ApplicationBuilderRedirects(app, options);

        return app;
    }
}