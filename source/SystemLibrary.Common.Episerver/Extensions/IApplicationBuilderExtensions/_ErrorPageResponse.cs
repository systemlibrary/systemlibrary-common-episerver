using System.Collections.Concurrent;
using System.Net;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

using SystemLibrary.Common.Net.Extensions;
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
                    path.StartsWith("/400", StringComparison.Ordinal) ||
                    path.StartsWith("/401", StringComparison.Ordinal) ||
                    path.StartsWith("/403", StringComparison.Ordinal) ||
                    path.StartsWith("/404", StringComparison.Ordinal) ||
                    path.StartsWith("/405", StringComparison.Ordinal) ||
                    path.StartsWith("/406", StringComparison.Ordinal) ||
                    path.StartsWith("/500", StringComparison.Ordinal) ||
                    path.StartsWith("/502", StringComparison.Ordinal) ||
                    path.StartsWith("/504", StringComparison.Ordinal) ||
                    path.StartsWith("/505", StringComparison.Ordinal))
                    return;

                if (path.IsFile()) return;

                var pathLowered = path.ToLower();

                if (pathLowered.StartsWith("/error", StringComparison.Ordinal)) return;

                if (pathLowered.StartsWith("/util/login", StringComparison.Ordinal) || pathLowered.StartsWith("/episerver/", StringComparison.Ordinal))
                {
                    if (pathLowered.Contains("stores/metadata/epi", StringComparison.Ordinal))
                        Log.Error(path + " not found, 404");

                    return;
                }

                if (pathLowered.StartsWithAny(StringComparison.Ordinal,"/favicon", "/.env", "/etc/")) return;

                if (pathLowered.Contains("wp-includes", StringComparison.Ordinal) ||
                    pathLowered.Contains("phpinfo", StringComparison.Ordinal) ||
                    pathLowered.StartsWith("/.aws/", StringComparison.Ordinal)) return;

                if (pathLowered.StartsWith("/globalassets/", StringComparison.Ordinal) ||
                    pathLowered.StartsWith("/contentassets/", StringComparison.Ordinal) ||
                    pathLowered.StartsWith("/siteassets/", StringComparison.Ordinal))
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

                    var json = JsonErrorResponseCache.Cache(statusCode, () =>
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

                var errorPages = BaseCms.GetLatestVersionOfContentType<IErrorPage>();

                if (errorPages == null) return;

                foreach (var errorPage in errorPages)
                {
                    if (errorPage?.StatusCodes?.Contains(statusCode) != true) continue;

                    if ((errorPage as IContent)?.IsPublished() != true) continue;

                    context.Response.Clear();

                    var errorPageType = errorPage.GetOriginalType();

                    var errorControllerType = ErrorControllerTypes.Cache(errorPageType.GetHashCode(), () =>
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