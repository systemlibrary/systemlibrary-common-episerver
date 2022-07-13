using System;

using Microsoft.AspNetCore.Builder;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Initialize;

public static partial class IApplicationBuilderExtensions
{
    public static IApplicationBuilder CommonEpiserverAppBuilder(this IApplicationBuilder app, IApplicationBuilderOptions options = null)
    {
        if (options == null)
            options = new IApplicationBuilderOptions();

        ApplicationBuilderLogging(app, options);

        if (!System.IO.File.Exists("module.config"))
        {
            throw new Exception("Module.config is not located at root, cannot continue with CommonEpiServer initialization. Remember to read and follow the Installation instructions. Install instructions are within the documentation for this nuget package");
        }

        ApplicationBuilderCompression(app, options);

        //MapContent() from Episerver calls UseRazorPages()
        options.UseRazorPagesEndpoints = false;

        app.CommonWebApplicationBuilder(options);

        ApplicationBuilderEndpoints(app, options);

        ApplicationBuilderRedirects(app, options);

        return app;
    }
}