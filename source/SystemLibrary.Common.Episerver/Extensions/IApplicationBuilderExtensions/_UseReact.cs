using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using React.AspNet;

using SystemLibrary.Common.Net.Configurations;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static void AddUseReact(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (options.ReactSsrScriptsInOrder.IsNot()) return;

        app.UseReact(config =>
        {
            config.SetLoadReact(false);
            config.SetLoadBabel(false);

            if (EnvironmentConfig.IsLocal)
            {
                config.SetReuseJavaScriptEngines(true);
                config.SetUseDebugReact(false);
                config.SetMaxUsagesPerEngine(options.ReactSsrMaxUsagesPerEngineLocal);
                config.SetAllowJavaScriptPrecompilation(false);
            }
            else
            {
                config.SetReuseJavaScriptEngines(true);
                config.SetUseDebugReact(false);
                config.SetMaxUsagesPerEngine(options.ReactSsrMaxUsagesPerEngineNonLocal);
                config.SetAllowJavaScriptPrecompilation(true);
            }

            foreach (var script in options.ReactSsrScriptsInOrder)
            {
                if (script.Is())
                {
                    if (script[0] == '~')
                        config.AddScriptWithoutTransform(script);
                    else
                        Log.Error("[IApplicationBuilderExtensions] options.ReactSsrScriptsInOrder contains an invalid path, they all must start with '~', yours is: " + script);
                }
            }
            config.ExceptionHandler = OnReactException;
        });
    }

    static void OnReactException(Exception arg1, string arg2, string arg3)
    {
        var message = arg1.Message + "\nComponent: " + arg2;

        Log.Error(message + " " + arg3 + "\n" + arg1.ToString());

        if (!EnvironmentConfig.IsProd)
        {
            if (HttpContextInstance.Current.Response != null)
            {
                if (!HttpContextInstance.Current.Response.HasStarted)
                {
                    HttpContextInstance.Current.Response.WriteAsync("<div class='" + Globals.CssClassName.SsrError + "' style='color:red;background-color:white;border-top:1px solid darkred; border-bottom:1px solid darkred;'>ServerSide: " + message.Replace("\"", "").Replace(Environment.NewLine, "<br>") + ".<br/>Tip: Check browsers console.<br/>Note: might need to restart APP to reload script changes<br/>Tip: Hide this error by css class " + Globals.CssClassName.SsrError + "</div>")
                      .ConfigureAwait(true)
                      .GetAwaiter()
                      .GetResult();
                }
            }
        }
    }
}