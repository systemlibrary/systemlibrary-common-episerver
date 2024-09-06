using System.Collections.Concurrent;
using System.Net;
using System.Reflection;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

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
        if (!options.UseErrorPageResponse) return;

        app.Use(async (context, next) =>
        {
            await next();

            var statusCode = context?.Response?.StatusCode ?? 0;

            if (statusCode <= 399 || statusCode == 503) return;

            if (BaseCms.IsInPreviewMode || BaseCms.IsInEditMode) return;

            Debug.Log("Error page response invoked on status " + statusCode);

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

            if (pathLowered.StartsWithAny(StringComparison.Ordinal, "/favicon", "/.env", "/etc/")) return;

            if (pathLowered.Contains("wp-includes", StringComparison.Ordinal) ||
                pathLowered.Contains("phpinfo", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/.aws/", StringComparison.Ordinal)) return;

            if (pathLowered.StartsWith("/globalassets/", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/contentassets/", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/siteassets/", StringComparison.Ordinal))
            {
                // TODO: Consider returning a dummy image, blank...
                return;
            }

            var isAjaxRequest = context.Request.IsAjaxRequest();
            var expectJsonResponse = isAjaxRequest;
            var expectXmlResponse = false;

            if (!expectJsonResponse)
            {
                var accept = context.Request.Headers["Accept"].FirstOrDefault();
                if (accept == null)
                    accept = context.Request.Headers["accept"].FirstOrDefault();
                if (accept == null)
                    accept = context.Request.Headers["ACCEPT"].FirstOrDefault();

                if (accept != null)
                {
                    expectJsonResponse = accept.ToLower().Contains("application/json");

                    if (!expectJsonResponse)
                    {
                        expectXmlResponse = accept.ToLower().Contains("application/xml");
                    }
                }
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

                await context.Response.WriteAsync(json).ConfigureAwait(false);
                return;
            }

            // Only returned in a non-ajax request for now
            if (expectXmlResponse)
            {
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync("<error><statusCode>" + statusCode + "</statusCode><statusMessage>" + ((HttpStatusCode)statusCode).ToString() + "</statusMessage></error>").ConfigureAwait(false);
                return;
            }

            // TODO: Consider adding some kind of cache for some of the status codes based on path? only if query is blank?
            //var relativeUrl = (pathLowered + context?.Request?.QueryString);
            //var viewCacheKey = nameof(ErrorPageResponse) + "ViewResult" + statusCode + (relativeUrl.GetHashCode() % 100).ToString() + Math.Min(relativeUrl.Length, 64);
            //var cachedViewData = Cache.Get<object[]>(viewCacheKey);
            //if (cachedViewData != null)
            //{
            //    var routeData = new RouteData();
            //    routeData.Values["controller"] = cachedViewData[1].ToString();
            //    var actionContext = new ActionContext(context, routeData, new ControllerActionDescriptor());
            //    var view = cachedViewData[0] as ViewResult;
            //    await view.ExecuteResultAsync(actionContext).ConfigureAwait(false);
            //    return;
            //}

            // TODO: Cache if request is a simple GET for a URL, less than 12chars in path?
            // TODO: Cache first 100 errors, that are GET for a URL?

            var errorPagesCacheKey = "SysLibEpiErrorPagesList" + nameof(IApplicationBuilderExtensions) + nameof(ErrorPageResponse);

            var errorPages = Cache.Get<List<IErrorPage>>(errorPagesCacheKey);
            if (errorPages == null)
            {
                errorPages = BaseCms.GetAllLatestVersionsOfContentType<IErrorPage>().ToList();
                Cache.Set(errorPagesCacheKey, errorPages, TimeSpan.FromSeconds(600));
            }

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
                    await context.Response.WriteAsync("Controller not found, namespace + name: " + errorPageType.Namespace + "." + errorPageType.Name + ". Controller do not exist. Please create it as your 'error page' must have a controller, and double check the namespace.").ConfigureAwait(false);
                }
                else
                {
                    var controller = Activator.CreateInstance(errorControllerType) as ControllerBase;

                    if (controller == null)
                    {
                        await context.Response.WriteAsync("Controller was found, but it does not inherit ControllerBase: " + errorControllerType.Name).ConfigureAwait(false);
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
                            await context.Response.WriteAsync("Controller must have a Index method that takes the 'error page' as first argument, the second argument is the erroring request path").ConfigureAwait(false);
                        }
                        else
                        {
                            try
                            {
                                var view = index.Invoke(controller, [errorPage, context?.Request?.Url()]) as ViewResult;

                                // Cache.Set(viewCacheKey, new object[] { view, errorPageType.Name }, TimeSpan.FromSeconds(90));

                                await view.ExecuteResultAsync(actionContext).ConfigureAwait(false);
                            }
                            catch (TargetParameterCountException e)
                            {
                                var msg = errorControllerType.Name + " and the Index method, does not have the right parameters. The first param should be 'currentPage' of your Error Page Type, and second parameter should be a string 'url'. " + e.Message;
                                
                                Log.Error(msg);

                                await context.Response.WriteAsync(msg).ConfigureAwait(false);

                                //throw new Exception(errorControllerType.Name + " and the Index method, does not have the right parameters. The first param should be 'currentPage' of your Error Page Type, and second parameter should be a string 'url'", e);
                            }
                        }
                    }
                }
            }
        });
    }
}