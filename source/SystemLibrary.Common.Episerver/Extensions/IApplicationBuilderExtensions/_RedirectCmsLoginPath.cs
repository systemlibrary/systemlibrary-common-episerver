using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void RedirectCmsLoginPath(this IApplicationBuilder app, CmsFrameworkOptions options)
    {
        var path = options.CmsLoginPath;

        if (path.IsNot()) return;

        if (path.StartsWith("/"))
            path = path.Substring(1);

        app.UseRewriter(new RewriteOptions()
            .AddRedirect(path.ToPascalCase() + "$", "episerver/cms/")
            .AddRedirect(path.toCamelCase() + "$", "episerver/cms/")
            .AddRedirect(path.ToPascalCase() + "/$", "episerver/cms/")
            .AddRedirect(path.toCamelCase() + "/$", "episerver/cms/"));
    }
}
