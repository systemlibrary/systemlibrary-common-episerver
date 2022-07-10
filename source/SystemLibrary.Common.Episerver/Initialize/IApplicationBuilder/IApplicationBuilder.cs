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

        app.CommonWebApplicationBuilder(options);

        ApplicationBuilderEndpoints(app, options);

        ApplicationBuilderRedirects(app, options);

        ApplicationBuilderCompression(app, options);

        return app;
    }
}