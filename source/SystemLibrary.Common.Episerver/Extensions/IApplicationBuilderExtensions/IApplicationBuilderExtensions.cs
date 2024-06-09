using System;
using System.IO;
using System.Net;

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
    ///     app.UseCommonCmsApp();
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

        var useControllers = options.UseControllers;
        var useRazorPages = options.UseRazorPages;
        var useApiControllers = options.UseApiControllers;

        // Disable all endpoints in "CommonWeb", OptimizelyCMS is picky in the order
        // and for "performance" we want to register Content early
        options.UseControllers = false;
        options.UseRazorPages = false;
        options.UseApiControllers = false;

        app.UseCommonWebApp(env, options);

        options.UseControllers = useControllers;
        options.UseRazorPages = useRazorPages;
        options.UseApiControllers = useApiControllers;

        app.UseMapEndpoints(options);

        app.AddUseReact(options);

        app.RedirectCmsLoginPath(options);

        app.ErrorPageResponse(options);
        
        return app;
    }
}