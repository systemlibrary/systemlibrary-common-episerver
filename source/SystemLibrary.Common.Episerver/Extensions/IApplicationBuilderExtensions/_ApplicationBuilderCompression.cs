using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderCompression(IApplicationBuilder app, CommonEpiserverApplicationBuilderOptions options)
    {
        app.UseResponseCompression();
    }
}