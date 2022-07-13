﻿using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

using EPiServer.Logging;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;

using static SystemLibrary.Common.Episerver.AppSettings.Configuration;

namespace SystemLibrary.Common.Episerver
{
    partial class Log
    {
        static class LogMessageBuilder
        {
            static bool IsLocal;
            static LogMessageBuilderOptions LogMessageBuilderOptions;
            static LogMessageBuilder()
            {
                IsLocal = EnvironmentConfig.Current.IsLocal;
                LogMessageBuilderOptions = AppSettings.Current.SystemLibraryCommonEpiserver.LogMessageBuilder;
            }
             
            static IPageRouteHelper _PageRouteHelper;
            static IPageRouteHelper PageRouteHelper => _PageRouteHelper != null ?
                _PageRouteHelper :
                (_PageRouteHelper = Services.Get<IPageRouteHelper>());

            internal static string Get(object obj, Level level)
            {
                var message = new StringBuilder("");

                if (level != (Level)1000)
                    message.Append(level.ToString() + ": ");

                var context = HttpContextInstance.Current;

                AppendMessage(obj, message);
                AppendRequestPath(message, context?.Request);

                if (!IsLocal)
                {
                    if(LogMessageBuilderOptions.AppendCurrentPage)
                        AppendCurrentPage(message);

                    if(LogMessageBuilderOptions.AppendLoggedInState)
                        AppendLoggedInState(message, context);

                    if (LogMessageBuilderOptions.AppendBrowser)
                        AppendBrowser(message, context?.Request);

                    if (LogMessageBuilderOptions.AppendIp)
                        AppendUserIp(message, context);

                    if (LogMessageBuilderOptions.AppendCookieInfo)
                        AppendCookieInfo(message, context?.Request);
                }

                return message.ToString();
            }

            static void AppendUserIp(StringBuilder message, HttpContext httpContext)
            {
                var connection = httpContext?.Connection;
                var remoteIpAddress = connection?.RemoteIpAddress;
                var localIpAddress = connection?.LocalIpAddress;

                if (remoteIpAddress == null && localIpAddress == null)
                {
                    message.Append("\nUser Ip: ");
                    return;
                }
                //httpContext?.Request?.Headers["HTTP_X_FORWARDED_FOR"];
                //httpContext?.Request?.Headers["REMOTE_ADDR"];

                var part1 = localIpAddress?.ToString();
                var part2 = remoteIpAddress?.ToString();

                if (part1.IsNot())
                    message.Append("\nUser Ip: " + part2);

                else if (part2.IsNot())
                    message.Append("\nUser Ip: " + part1);
                else
                    message.Append("\nUser Ip: " + part1 + ", " + part2);
            }

            static void AppendLoggedInState(StringBuilder message, HttpContext httpContext)
            {
                var isAuthenticated = httpContext?.User?.Identity?.IsAuthenticated == true;

                message.Append("\nIsLoggedIn: " + isAuthenticated);
            }

            static void AppendCookieInfo(StringBuilder message, HttpRequest request)
            {
                if(request?.Cookies?.Keys != null)
                {
                    message.Append("\nCookies: " + string.Join(", ", request.Cookies.Keys));
                    var maxCookieValueLength = 0;
                    var maxCookieName = "";
                    foreach (var key in request.Cookies.Keys)
                    {
                        var value = request.Cookies[key];
                        if(value.Is() && value.Length > maxCookieValueLength)
                        {
                            maxCookieName = key;
                            maxCookieValueLength = value.Length;
                        }
                    }
                    if(maxCookieValueLength > 0)
                        message.Append("\nCookies Max Length: " + maxCookieValueLength + " chars, in cookie: " + maxCookieName);
                }
            }

            static void AppendMessage(object obj, StringBuilder message)
            {
                if (obj == null)
                    message.Append("(null)");

                else if (obj is Exception ex)
                {
                    message.Append(ex.Message);
                    message.Append("\n" + ex.StackTrace);
                }

                else if (obj is ITuple ituple)
                    message.Append(ituple[0] + ", " + (ituple?.Length > 1 ? ituple[1] + "" : ""));

                else if (obj is string txt)
                    message.Append(txt);

                else if (obj is IEnumerable enumerable)
                {
                    if (obj is IList list)
                        message.Append("List (" + list.Count + "): ");
                    if (obj is Array array)
                        message.Append("Array (" + array.Length + "): ");
                    if (obj is ICollection collection)
                        message.Append("Collection (" + collection.Count + "): ");

                    if (obj is IDictionary dictionary)
                    {
                        message.Append("Dictionary (" + dictionary.Count + "): ");
                        foreach (var value in dictionary.Values)
                            message.Append(value + " ");
                    }
                    else
                    {
                        foreach (var val in enumerable)
                            message.Append(val + " ");
                    }
                }
                else
                    message.Append(obj.ToString());
            }

            static void AppendBrowser(StringBuilder message, HttpRequest request)
            {
                string userAgent = null;

                if (request?.Headers?.ContainsKey(HeaderNames.UserAgent) == true)
                    userAgent = request.Headers[HeaderNames.UserAgent];

                message.Append("\nAgent: " + userAgent ?? "<empty>");
            }

            static void AppendRequestPath(StringBuilder message, HttpRequest request)
            {
                message.Append("\nPath: " + request?.Path.Value ?? "<empty>");
            }

            static void AppendCurrentPage(StringBuilder message)
            {
                var content = PageRouteHelper?.Content;

                if (content != null)
                    message.Append("\nPage: " + content.Name + " (id, " + content.ContentLink?.ID + ")");
                else
                    message.Append("\nPage: <none>");
            }
        }
    }
}
