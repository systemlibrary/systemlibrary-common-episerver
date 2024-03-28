using System;
using System.IO;

using EPiServer.Events.Clients;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using React.AspNet;

using SystemLibrary.Common.Episerver.Components;
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
    public static IApplicationBuilder UseCommonEpiserverApp(this IApplicationBuilder app, IWebHostEnvironment env, UseCommonEpiserverAppOptions options = null)
    {
        if (!File.Exists("module.config"))
        {
            throw new Exception("Module.config is not located at root, cannot continue with Common Episerver Initialization. Remember: follow the instructions at https://systemlibrary.github.io/systemlibrary-common-episerver/Install.html");
        }

        IWebHostEnvironmentInstance.Initialize(env);

        if (options == null)
            options = new UseCommonEpiserverAppOptions();

        DefaultBlockComponent.DefaultComponentPathPredicate = options.DefaultComponentPathPredicate;

        //app.Use(async (context, next) =>
        //{
        //    await next();

        //    var path = context?.Request?.Path.Value;

        //    if (path != null)
        //    {
        //        var l = path.Length;
        //        if (l > 4 && path[l - 1] != '/' && !path.EndsWith(".js") && !path.EndsWith(".png") && !path.EndsWith(".jpg") && !path.EndsWith(".css"))
        //        {
        //            try
        //            {
        //                if (path.EndsWithAnyCaseInsensitive(".dll", ".cs", ".cshtml", ".config"))
        //                {
        //                    return;
        //                }

        //                if (l > 13)
        //                {
        //                    if (path[1] == 'a' || path[1] == 'A' ||
        //                        path[1] == 'c' || path[1] == 'C')
        //                    {
        //                        var p = path.ToLower();
        //                        if (p.StartsWith("/appsettings.") ||
        //                                p.StartsWith("/configurations/") ||
        //                                p.StartsWith("/configuration/") ||
        //                                p.StartsWith("/config/") ||
        //                                p.StartsWith("/configs/"))
        //                        {
        //                            return;
        //                        }
        //                    }
        //                }

        //                if (l > 7)
        //                {
        //                    if (path[1] == 'b' || path[1] == 'B')
        //                    {
        //                        var p = path.ToLower();
        //                        if (p.StartsWith("/bin/"))
        //                        {
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Log.Error(ex);
        //            }
        //        }

        //        if(context.Response.StatusCode == 404 && path.StartsWith("/SystemLibrary/CommonEpiserverCms/"))
        //        {
        //            context.Response.StatusCode = 200;
        //            await context.Response.WriteAsync("");
        //        }
        //    }
        //});

        ApplicationBuilderLogging(app, options);

        app.CommonWebApplicationBuilder(options);

        ApplicationBuilderEndpoints(app, options);

        app = app.UseReact(config =>
        {
            config.SetLoadReact(false);
            config.SetLoadBabel(false);

            config.AddScriptWithoutTransform("~/Static/webpack3/dist/vendor-react-webpack3.js");
            config.AddScriptWithoutTransform("~/Static/webpack3/dist/vendors-webpack3.js");
            config.AddScriptWithoutTransform("~/Static/webpack3/dist/index-webpack3.js");

            config.SetReuseJavaScriptEngines(false);
            config.SetUseDebugReact(false);
            config.SetMaxUsagesPerEngine(1);
            config.SetAllowJavaScriptPrecompilation(false);
            //config.UseServerSideRendering = true;

            //config.ExceptionHandler = OnReactRenderingException;

            Dump.Write("Using react");
        });

        app.UseWhen((httpContext) => (httpContext.Response?.HasStarted == true && env != null && env.WebRootPath == null && WebApplicationInitializer.HasInitialized == true), appBuilder =>
        {
            Dump.Write("Settings WebRootPath as its null, after the initialization was completed: "  + WebApplicationInitializer.HasInitialized);
            // NOTE: Fallthrough three times, in case bin/release/version 

            //env.WebRootPath = new DirectoryInfo(AppContext.BaseDirectory).FullName;
            //if (env.WebRootPath.Contains("\\bin\\"))
            //    env.WebRootPath = new DirectoryInfo(env.WebRootPath).Parent.FullName;
            //if (env.WebRootPath.Contains("\\bin\\"))
            //    env.WebRootPath = new DirectoryInfo(env.WebRootPath).Parent.FullName;
            //if (env.WebRootPath.Contains("\\bin\\"))
            //    env.WebRootPath = new DirectoryInfo(env.WebRootPath).Parent.FullName;
        });


        //ApplicationBuilderRedirectCmsLoginPath(app, options);


        //       if (env.WebRootPath == null)
        //{
        //    // NOTE: Fallthrough three times, in case bin/release/version 
        //    env.WebRootPath = new DirectoryInfo(AppContext.BaseDirectory).FullName;
        //    if (env.WebRootPath.Contains("\\bin\\"))
        //        env.WebRootPath = new DirectoryInfo(env.WebRootPath).Parent.FullName;
        //    if (env.WebRootPath.Contains("\\bin\\"))
        //        env.WebRootPath = new DirectoryInfo(env.WebRootPath).Parent.FullName;
        //    if (env.WebRootPath.Contains("\\bin\\"))
        //        env.WebRootPath = new DirectoryInfo(env.WebRootPath).Parent.FullName;
        //}
        return app;
    }
}

public static class IWebHostEnvironmentInstance 
{
    static IWebHostEnvironment Instance;

    public static IWebHostEnvironment Current => Instance;

    internal static void Initialize(IWebHostEnvironment env)
    {
        Instance = env;
    }
}
