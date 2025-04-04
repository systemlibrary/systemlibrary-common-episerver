﻿using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

using EPiServer.Core;
using EPiServer.Web.Mvc.Html;

using Microsoft.AspNetCore.Html;

using Microsoft.AspNetCore.Mvc.Rendering;

using SystemLibrary.Common.Framework;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class HtmlHelperExtensions
{
    static string ExceptionPrefix = "Exception: ";

    /// <summary>
    /// Property for ContentArea renders all of the content area items
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static IHtmlContent PropertyForContentArea<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
        return html.PropertyFor(expression, new { SkipWrapperTag = true, HasContainer = false, HasItemContainer = false, CustomTag = "systemLibraryEpiserverContentAreaRender" });
    }

    /// <summary>
    /// Pass in the exception that occurs in rendering (in cshtml files).
    /// <para>- pass in either the exception, the string, or a tuple of the (Content Model, Exception)</para>
    /// </summary>
    /// <remarks>
    /// Logs exceptions that occurs if any
    /// <para>If environment is not "prod/production" it will add the error to the HTML Response</para>
    /// <para>Useful if you want to show errors in local, dev, QA, stage, but not in production, but still logging it</para>
    /// The div printed always has a class 'view-errored'
    /// </remarks>
    public static IHtmlContent ViewException<TModel>(this IHtmlHelper<TModel> html, object model)
    {
        static string Print(Exception ex)
        {
            var sb = new StringBuilder("", 512);

            sb.Append(ex?.Message ?? "");

            if (ex?.InnerException != null)
            {
                sb.Append(" ");
                sb.Append(ex.InnerException.Message);

                if (ex.InnerException.InnerException != null)
                {
                    sb.Append(" ");
                    sb.Append(ex.InnerException.InnerException.Message);
                    sb.Append(ex.InnerException.InnerException.StackTrace.MaxLength(255));
                    sb.Append(ex.StackTrace.MaxLength(255));
                }
                else
                {
                    sb.Append(ex.InnerException.StackTrace.MaxLength(255));
                    sb.Append(ex.StackTrace.MaxLength(255));
                }
            }
            else
            {
                sb.Append(" ");
                sb.Append(ex.StackTrace.MaxLength(512));
            }
            return sb.ToString();
        }

        var message = new StringBuilder("", 1024);

        IContent content = null;
        if (model == null)
        {
            message.Append("ViewException() got a model that is null, continuing silently...");
        }
        else if (model is Exception ex)
        {
            message.Append(Print(ex));
        }
        else
        {
            if (model is string txt)
            {
                message.Append(txt);
            }
            else if (model is ITuple tuple)
            {
                try
                {
                    var tupleModel = (ValueTuple<BlockData, Exception>)model;
                    content = tupleModel.Item1 as IContent;
                    message.Append(Print(tupleModel.Item2));
                }
                catch
                {
                    try
                    {
                        var tupleModel = (ValueTuple<PageData, Exception>)model;
                        content = tupleModel.Item1;
                        message.Append(Print(tupleModel.Item2));
                    }
                    catch
                    {
                        message.Append("Cannot generate proper error message as tuple is not (pagedata,ex) or (blockdata,ex): ");
                        message.Append(tuple.ToString());
                        // Swallow
                    }
                }
            }
            else if (model is ContentData contentModel)
            {
                message.Append("Error in content: " + (contentModel as IContent)?.Name);

                content = contentModel as IContent;
            }
            else
            {
                message.Append("ViewException() got a model that is not supported, continuing silently..." + model?.GetType().Name);
            }
        }

        Log.Error(message);

        if (!EnvironmentConfig.IsProd)
        {
            message = message.Replace("\r\n", "<br>");
            message = message.Replace("\n", "<br>");
            message = message.Replace("\r", "<br>");

            if (content?.ContentLink != null)
            {
                message.Append("<br/><br/><a style='color:blue;font-size: 14px;' target='_blank' href='/EPiServer/CMS/?#context=epi.cms.contentdata:///" + content.ContentLink.ID + "'>Link to content ID: " + content?.ContentLink.ID + "</a>");
            }
            message.Append("</div>");
        }

        // NOTE: Error ends with "</div>"
        var error = message.ToString();
        if (error.Contains("Object reference not set") && error.Contains("DisplayTemplate"))
        {
            error = "A call to View() returned null, make sure no controller/component returns null as 'the view', or a view file was not found for block/component/page. " + error;
        }

        if (error.Contains("was not found") && error.Contains("PartialContentController"))
        {
            error = "View is not found for a PartialContentController. If you are using 'BlockControllers' in C# 7 or newer, you must specify the full relative path to your block views, starting with ~/ and ending in .cshtml. Because there's a hardcoded path for all 'view components' (which block controllers use behind the scenes), that all views ends with Components/Default.cshtml. " + (model as Exception)?.Message;
        }

        return new HtmlString("<div class=\"" + Globals.CssClassName.ViewExceptionError + "\" style=\"font-size: 14px !important;min-width:320px;max-width:1920px;width:100%;color:darkred;background-color:white;border-top:1px solid red; border-bottom:1px solid red;\">" + ExceptionPrefix + error);
    }

    /// <summary>
    /// Display a blue edit link down right in the current container or light blue if level is 1 (nested)
    /// </summary>
    /// <param name="blockData">Block/component you want a short cut to edit for</param>
    /// <returns></returns>
    public static IHtmlContent ComponentEditLink<TModel>(this IHtmlHelper<TModel> html, BlockData blockData, int level = 0)
    {
        var link = Episerver.ComponentEditLink.Create(blockData as IContent, level);

        if (link == null)
            return HtmlString.Empty;

        return new HtmlString(link);
    }
}
