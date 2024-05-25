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
    static void AddUseReact(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (options.ReactSsrScriptsInOrder.IsNot()) return;

        app.UseReact(config =>
        {
            config.SetLoadReact(false);
            config.SetLoadBabel(false);

            if (EnvironmentConfig.IsLocal)
            {
                config.SetReuseJavaScriptEngines(false);
                config.SetUseDebugReact(false);
                config.SetMaxUsagesPerEngine(2);
                config.SetAllowJavaScriptPrecompilation(false);
            }
            else
            {
                config.SetReuseJavaScriptEngines(true);
                config.SetUseDebugReact(false);
                config.SetMaxUsagesPerEngine(2000);
                config.SetAllowJavaScriptPrecompilation(true);
            }

            foreach (var script in options.ReactSsrScriptsInOrder)
            {
                if (script.Is())
                {
                    if (script.StartsWith("~"))
                        config.AddScriptWithoutTransform(script);
                    else
                        Log.Error("React script path invalid, must start with ~. " + script);
                }
            }
            config.ExceptionHandler = OnReactException;
        });
    }

    static void OnReactException(Exception arg1, string arg2, string arg3)
    {
        var message = arg1.Message + " Component: " + arg2 ;

        Log.Error(message + arg3 + "\n" + arg1.ToString());

        if (!EnvironmentConfig.IsProd)
            HttpContextInstance.Current.Response.WriteAsync("<p style=\"color:red;background-color:white;border-top:1px solid darkred; border-bottom:1px solid darkred;\">ServerSide: " + message + ".<br/>Tip: Check your browsers console. <br/>Note: might need to restart app to reload changes</p>");
    }
}