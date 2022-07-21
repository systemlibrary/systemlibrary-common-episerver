using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderRedirects(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        if (!options.UseRewriteEpiserverPathToEpiserverCms) return;
        
        app.UseRewriter(new RewriteOptions()
            .AddRedirect("episerver$", "episerver/cms/")
            .AddRedirect("EPiServer$", "episerver/cms/")
            .AddRedirect("episerver/$", "episerver/cms/"));
    }
}