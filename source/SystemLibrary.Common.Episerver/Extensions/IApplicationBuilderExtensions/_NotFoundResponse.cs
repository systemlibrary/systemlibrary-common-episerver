using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

using React.AspNet;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void NotFoundResponse(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (options.UseNotFoundResponse) return;

        //app.UseWhen(context => context.Response.StatusCode == 404, appBuilder =>
        //{
        //    // app.Use(NotFoundMiddleware.Register());
        //});
    }
}