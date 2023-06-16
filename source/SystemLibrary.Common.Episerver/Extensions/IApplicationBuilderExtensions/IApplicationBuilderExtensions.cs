using System;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using SystemLibrary.Common.Episerver.Cms.Blocks;
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
    ///     app.CommonEpiserverAppBuilder();
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder CommonEpiserverApplicationBuilder(this IApplicationBuilder app, IWebHostEnvironment env, CommonEpiserverApplicationBuilderOptions options = null)
    {
        if (!File.Exists("module.config"))
        {
            throw new Exception("Module.config is not located at root, cannot continue with CommonEpiServer initialization. Remember to read and follow the instructions at:https://systemlibrary.github.io/systemlibrary-common-episerver/Install.html");
        }

        if (env.WebRootPath == null)
        {
            env.WebRootPath = new DirectoryInfo(AppContext.BaseDirectory).FullName;
            if (env.WebRootPath.EndsWith("\\bin\\"))
                env.WebRootPath = new DirectoryInfo(env.WebRootPath).Parent.FullName;
        }

        if (options == null)
            options = new CommonEpiserverApplicationBuilderOptions();

        DefaultBlockComponent.DefaultBlockComponentFolderPath = options.DefaultBlockComponentFolderPath;

        ApplicationBuilderLogging(app, options);

        app.Use(async (context, next) =>
        {
            var path = context?.Request?.Path.Value;
            if (path != null)
            {
                var l = path.Length;
                if (l > 3 && path[l-1] != '/')
                {
                    if (path.EndsWithAnyCaseInsensitive(".dll", ".cs", ".cshtml"))
                    {
                        return;
                    }

                    if(l > 13)
                    {
                        if (path[1] == 'a' || path[1] == 'A' ||
                            path[1] == 'c' || path[1] == 'C')
                        {
                            var p = path.ToLower();
                            if (p.StartsWith("/appsettings.") ||
                                    p.StartsWith("/configurations/") ||
                                    p.StartsWith("/configuration/") ||
                                    p.StartsWith("/config/") ||
                                    p.StartsWith("/configs/"))
                            {
                                return;
                            }
                        }
                    }
                    else if (l > 6)
                    {
                        if(path[1] == 'b' || path[1] == 'B')
                        {
                            var p = path.ToLower();
                            if (p.StartsWith("/bin/")) return;
                        }
                    }
                }
            }
            await next();
        });

        app.CommonWebApplicationBuilder(options);

        ApplicationBuilderEndpoints(app, options);

        ApplicationBuilderRedirectCmsLoginPath(app, options);

        return app;
    }
}