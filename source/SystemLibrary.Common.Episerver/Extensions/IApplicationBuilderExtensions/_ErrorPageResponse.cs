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

            var path = context?.Request?.Path.HasValue == true ? context.Request.Path.Value : null;

            if (path.IsNot()) return;

            var length = path.Length;

            // No response for "/" nor "/a"
            if (length <= 2) return;

            if (length <= 4)
            {
                // No response for "/10", nor "/404", nor "/500"
                if (char.IsDigit(path[1]) && char.IsDigit(path[2]))
                {
                    return;
                }
            }

            // TODO Add option for a default Image, if set, we return that image on 404/500/502/504 for Images
            if (path.IsFile())
            {
                if (path.EndsWith(".js") || path.Contains(".js?"))
                {
                    context.Response.ContentType = "application/javascript; charset=utf-8";
                    await context.Response.WriteAsync("// 404").ConfigureAwait(false);
                }

                else if (path.EndsWith(".svg"))
                {
                    context.Response.ContentType = "image/svg+xml";
                    await context.Response.WriteAsync("<svg viewBox=\"0 0 1 1\"><text>404</text></svg>").ConfigureAwait(false);
                }

                else if (path.EndsWith(".css") || path.Contains(".css?"))
                {
                    context.Response.ContentType = "text/css; charset=utf-8";
                    await context.Response.WriteAsync("/* 404 */").ConfigureAwait(false);
                }

                else if (path.EndsWith(".txt") || path.Contains(".txt?"))
                {
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("404").ConfigureAwait(false);
                }

                else if(path.EndsWith(".png") || path.Contains(".png?"))
                {
                    var transparentPngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkqAcAAIUAgUW0RjgAAAAASUVORK5CYII=";

                    var imageBytes = Convert.FromBase64String(transparentPngBase64);

                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "image/png";
                    await context.Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length).ConfigureAwait(false);
                }

                else if(path.EndsWith(".jpg") || path.EndsWith(".jpg?"))
                {
                    var whiteJpegBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAACNbyblAAAAAElFTkSuQmCC";

                    var imageBytes = Convert.FromBase64String(whiteJpegBase64);

                    context.Response.ContentType = "image/jpeg";
                    context.Response.StatusCode = 404;
                    await context.Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length);
                }

                return;
            }


            var pathLowered = path.ToLower();

            // Paths we just ignore errors on
            if (pathLowered.StartsWith("/error", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/recycle-bin/", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/.aws/", StringComparison.Ordinal) ||
                pathLowered.StartsWithAny(StringComparison.Ordinal, "/favicon", "/.env", "/etc/") ||
                pathLowered.StartsWith("/util/", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/globalassets/", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/contentassets/", StringComparison.Ordinal) ||
                pathLowered.StartsWith("/siteassets/", StringComparison.Ordinal)) return;

            // Paths we want to log errors on
            if (pathLowered.StartsWith("/episerver/", StringComparison.Ordinal))
            {
                if (pathLowered.Contains("/metadata/", StringComparison.Ordinal))
                    Log.Error(path + " not found, 404");

                return;
            }

            // Paths we want to log debug info on
            if (pathLowered.StartsWith("/systemlibrary"))
            {
                Debug.Log(pathLowered + " " + statusCode);
                return;
            }

            // Paths that contains we just ignore
            if (pathLowered.Contains("wp-includes", StringComparison.Ordinal) ||
                pathLowered.Contains("phpinfo", StringComparison.Ordinal) ||
                pathLowered.EndsWith(".environment", StringComparison.Ordinal)) return;

            var isAjaxRequest = context.Request.IsAjaxRequest();

            // Ajax request towards Controllers for Pages, Blocks, Components, do nothing
            if (isAjaxRequest)
            {
                if (pathLowered.StartsWith("/content/", StringComparison.Ordinal)) return;
                if (pathLowered.StartsWith("/controllers/", StringComparison.Ordinal)) return;
                if (pathLowered.StartsWith("/pages/", StringComparison.Ordinal)) return;
                if (pathLowered.StartsWith("/blocks/", StringComparison.Ordinal)) return;
                if (pathLowered.StartsWith("/components/", StringComparison.Ordinal)) return;
            }

            var expectJsonResponse = isAjaxRequest;
            var expectXmlResponse = false;
            var expectHtmlResponse = false;

            if (!expectJsonResponse)
            {
                var accept = context.Request.Headers["Accept"].FirstOrDefault();
                if (accept == null)
                    accept = context.Request.Headers["accept"].FirstOrDefault();
                if (accept == null)
                    accept = context.Request.Headers["ACCEPT"].FirstOrDefault();

                if (accept != null)
                {
                    expectJsonResponse = accept.ToLower() == "application/json";

                    if (!expectJsonResponse)
                    {
                        expectXmlResponse = accept.ToLower() == "application/xml";

                        if (!expectXmlResponse)
                        {
                            expectHtmlResponse = accept == "text/html";
                        }
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
            else if (expectXmlResponse)
            {
                context.Response.ContentType = "application/xml";
                await context.Response.WriteAsync("<error><statusCode>" + statusCode + "</statusCode><statusMessage>" + ((HttpStatusCode)statusCode).ToString() + "</statusMessage></error>").ConfigureAwait(false);
                return;
            }
            else if (expectHtmlResponse)
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<div class='" + Globals.CssClassName.HtmlErrorResponse + "'>" + statusCode + ": " + ((HttpStatusCode)statusCode).ToString() + "</div>").ConfigureAwait(false);
                return;
            }

            // TODO: Consider adding some kind of cache for some of the status codes based on path? only if query is blank?
            // Add first 50 error responses to a Cache, any other is always recalc'd, but that means we easily serve top most errors rapidly
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

            if (errorPages == null)
            {
                Debug.Log("UseErrorPageResponse is 'true', but no error pages were found");
                return;
            }

            foreach (var errorPage in errorPages)
            {
                if (errorPage?.StatusCodes?.Contains(statusCode) != true) continue;

                if ((errorPage as IContent)?.IsPublished() != true) continue;

                context.Response.Clear();
                context.Response.StatusCode = statusCode;

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
                            }
                        }
                    }
                }
            }
        });
    }
}
