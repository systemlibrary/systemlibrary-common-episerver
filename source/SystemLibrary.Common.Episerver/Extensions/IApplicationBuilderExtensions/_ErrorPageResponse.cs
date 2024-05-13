using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Text.Encodings.Web;

using Azure;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Licensing.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using React.AspNet;

using SystemLibrary.Common.Episerver.Cms;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class IApplicationBuilderExtensions
{
    static ConcurrentDictionary<int, string> JsonErrorResponseCache = new ConcurrentDictionary<int, string>();
    static ConcurrentDictionary<int, Type> ErrorControllerTypes = new ConcurrentDictionary<int, Type>();

    static void ErrorPageResponse(this IApplicationBuilder app, CmsAppBuilderOptions options)
    {
        if (options.UseErrorPageResponse) return;

        app.Use(async (context, next) =>
        {
            await next();

            try
            {
                var statusCode = context?.Response?.StatusCode ?? 0;

                if (statusCode <= 399 || statusCode == 503) return;

                if (BaseCms.IsInPreviewMode || BaseCms.IsInEditMode) return;

                var path = context?.Request?.Path.HasValue == true ? context.Request.Path.Value : null;

                if (path.IsNot()) return;

                if (path == "/" ||
                    path.StartsWith("/400") ||
                    path.StartsWith("/401") ||
                    path.StartsWith("/403") ||
                    path.StartsWith("/404") ||
                    path.StartsWith("/405") ||
                    path.StartsWith("/406") ||
                    path.StartsWith("/500") ||
                    path.StartsWith("/502") ||
                    path.StartsWith("/504") ||
                    path.StartsWith("/505"))
                    return;

                if (path.IsFile()) return;

                var pathLowered = path.ToLower();

                if (pathLowered.StartsWith("/error")) return;

                if (pathLowered.StartsWith("/util/login") || pathLowered.StartsWith("/episerver/"))
                {
                    if (pathLowered.Contains("stores/metadata/epi"))
                        Log.Error(path + " not found, 404");

                    return;
                }

                if (pathLowered.StartsWithAny("/favicon", "/.env", "/etc/")) return;

                if (pathLowered.Contains("wp-includes") ||
                    pathLowered.Contains("phpinfo") ||
                    pathLowered.StartsWith("/.aws/")) return;

                if (pathLowered.StartsWith("/globalassets/") ||
                    pathLowered.StartsWith("/contentassets/") ||
                    pathLowered.StartsWith("/siteassets/"))
                {
                    return;
                }

                var expectJsonResponse = context.Request.IsAjaxRequest();

                if (!expectJsonResponse)
                {
                    var accept = context.Request.Headers["Accept"].FirstOrDefault();
                    if (accept == null)
                        accept = context.Request.Headers["accept"].FirstOrDefault();
                    if (accept == null)
                        accept = context.Request.Headers["ACCEPT"].FirstOrDefault();

                    if (accept != null)
                        expectJsonResponse = accept.ToLower().Contains("application/json");
                }

                if (expectJsonResponse)
                {
                    context.Response.ContentType = "application/json";

                    var json = JsonErrorResponseCache.TryGet(statusCode, () =>
                    {
                        var json = new
                        {
                            statusCode = statusCode,
                            statusMessage = ((HttpStatusCode)statusCode).ToString(),
                        };

                        return json.Json();
                    });

                    context.Response.WriteAsync(json);
                    return;
                }

                var errorPages = BaseCms.GetLatestContentDataOfType<IErrorPage>();

                if (errorPages == null) return;

                foreach (var errorPage in errorPages)
                {
                    if (errorPage?.StatusCodes?.Contains(statusCode) != true) continue;

                    if ((errorPage as IContent)?.IsPublished() != true) continue;

                    context.Response.Clear();

                    var errorPageType = errorPage.GetOriginalType();

                    var errorControllerType = ErrorControllerTypes.TryGet(errorPageType.GetHashCode(), () =>
                    {
                        return Type.GetType(errorPageType.Namespace + "." + errorPageType.Name + "Controller, " + errorPageType.Assembly.FullName);
                    });

                    if (errorControllerType == null)
                    {
                        await context.Response.WriteAsync("Controller not found: " + errorPageType.Namespace + "." + errorPageType.Name + "Controller do not exist. Please create it as your 'error page' must have a controller");
                    }
                    else
                    {
                        var controller = Activator.CreateInstance(errorControllerType) as ControllerBase;

                        if (controller == null)
                        {
                            await context.Response.WriteAsync("Controller was found, but wrong type, must be a ControllerBase: " + errorControllerType.Name);
                        }
                        else
                        {
                            var routeData = new RouteData();
                            routeData.Values["controller"] = errorPageType.Name;

                            var actionContext = new ActionContext(context, routeData, new ControllerActionDescriptor());
                            controller.ControllerContext = new ControllerContext(actionContext);

                            var index = errorControllerType.GetMethod("Index");

                            if (index == null)
                            {
                                await context.Response.WriteAsync("Controller must have a Index method that takes the 'error page' as first argument, the second argument is the erroring request path");
                            }
                            else
                            {
                                var view = index.Invoke(controller, new object[] { errorPage, path }) as ViewResult;

                                await view.ExecuteResultAsync(actionContext);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Swallow
            }
        });
    }
}