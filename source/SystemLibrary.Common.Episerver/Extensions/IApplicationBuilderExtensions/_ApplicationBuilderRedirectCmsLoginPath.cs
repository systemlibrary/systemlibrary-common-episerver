using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderRedirectCmsLoginPath(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        var path = options.CmsLoginPath;

        if (path.IsNot()) return;

        app.UseRewriter(new RewriteOptions()
            .AddRedirect(path.ToPascalCase() + "$", "episerver/cms/")
            .AddRedirect(path.toCamelCase() + "$", "episerver/cms/"));
    }
}
