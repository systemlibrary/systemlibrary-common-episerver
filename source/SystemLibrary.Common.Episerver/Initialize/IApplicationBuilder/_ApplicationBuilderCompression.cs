using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Episerver.Initialize;

partial class IApplicationBuilderExtensions
{
    static void ApplicationBuilderCompression(IApplicationBuilder app, IApplicationBuilderOptions options)
    {
        app.UseResponseCompression();
    }
}