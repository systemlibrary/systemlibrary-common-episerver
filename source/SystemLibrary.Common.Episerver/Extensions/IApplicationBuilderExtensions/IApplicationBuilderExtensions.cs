using EPiServer;
using EPiServer.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using SystemLibrary.Common.Episerver.Components;
using SystemLibrary.Common.Framework.App.Extensions;

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
    public static IApplicationBuilder UseCommonCmsApp(this IApplicationBuilder app, IWebHostEnvironment env, CmsFrameworkOptions options = null)
    {
        if (!File.Exists("module.config"))
        {
            throw new Exception("Module.config is not located at root using File.Exists('module.config'), cannot continue with Common Episerver Initialization. Remember: follow the instructions at https://systemlibrary.github.io/systemlibrary-common-episerver/Install.html");
        }

        options ??= new CmsFrameworkOptions();

        _DefaultBlockComponent.DefaultComponentPathPredicate = options.DefaultComponentPathPredicate;

        app.SkipStaticLibraryRequestErrors(options);

        app.ExceptionHandler(options);

        if (options.UseApplicationCookieMaxSessionDuration)
            app.UseMiddleware<ApplicationCookieMaxSessionDurationMiddleware>();
        
        options.UseMvc = false;

        options.BeforeDefaultEndpoints = CmsPrecededEndpoints;

        app.UseFrameworkMiddlewares(env, options);

        app.UseMapEndpoints();

        app.AddUseReact(options);

        app.RedirectCmsLoginPath(options);

        app.ErrorPageResponse(options);

        return app;
    }
}

// TODO: Is this even in use? How?
public class WarmupHostedService : IHostedService
{
    readonly IContentLoader _contentLoader;

    public WarmupHostedService(IContentLoader contentLoader) => _contentLoader = contentLoader;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Preload Start Page
        var startPage = _contentLoader.Get<PageData>(ContentReference.StartPage);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}