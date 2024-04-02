using System;
using System.IO;

using EPiServer.Events.Clients;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;

using React.AspNet;

using SystemLibrary.Common.Episerver.Components;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Application Builder Extensions
/// </summary>
public static partial class IApplicationBuilderExtensions
{
    /// <summary>
    /// Initialize middleware for your Episerver Application in one line
    /// - Pass in 'EpiserverAppBuilderOptions' if you want to control which middleware to load
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///     app.UseCommonEpiserverApp();
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder UseCommonCmsApp(this IApplicationBuilder app, IWebHostEnvironment env, CmsAppBuilderOptions options = null)
    {
        if (!File.Exists("module.config"))
        {
            throw new Exception("Module.config is not located at root, cannot continue with Common Episerver Initialization. Remember: follow the instructions at https://systemlibrary.github.io/systemlibrary-common-episerver/Install.html");
        }

        if (options == null)
            options = new CmsAppBuilderOptions();

        DefaultBlockComponent.DefaultComponentPathPredicate = options.DefaultComponentPathPredicate;

        app.DisallowKnownAppFiles(options);

        app.ExceptionHandler(options);

        app.UseCommonWebApp(env, options);

        app.AddUseReact(options);

        app.UseEndpoints(options);

        app.RedirectCmsLoginPath(options);

        return app;
    }
}

