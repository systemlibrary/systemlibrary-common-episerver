using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

using EPiServer.Core;
using EPiServer.Shell.Navigation;
using EPiServer.Web.Mvc.Html;

using Microsoft.AspNetCore.Html;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class HtmlHelperExtensions
{
    public static IHtmlContent PropertyForContentArea<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
        return html.PropertyFor(expression, new { SkipWrapperTag = true, HasContainer = false, HasItemContainer = false, CustomTag = "systemLibraryEpiserverContentAreaRender" });
    }

    static string ExceptionPrefix = "Exception: ";

    /// <summary>
    /// Pass in the exception that occurs in rendering (in cshtml files).
    /// - pass in either the exception, the string, or a tuple of the (Content Model, Exception)
    /// 
    /// This logs the error that occured
    /// 
    /// And if the environment is not "prod" it will display the error in HTML
    /// </summary>
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

        if (model is Exception ex)
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
                        content = tupleModel.Item1 as IContent;
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
            message = message.Replace(System.Environment.NewLine, "<br/>");

            if (content?.ContentLink != null)
            {
                message.Append("<br/><a style='color:blue;' target='_blank' href='/EPiServer/CMS/?#context=epi.cms.contentdata:///" + content.ContentLink.ID + "'>Erroring content " + content?.ContentLink.ID + "</a>");
            }
            message.Append("</div>");
        }

        var error = message.ToString();
        if (error.Contains("Object reference not set") && error.Contains("DisplayTemplate"))
        {
            error = "A call to View() returned null, make sure no controller/component returns null as 'the view', or a view file was not found for block/component/page. " + error;
        }

        return new StringHtmlContent("<div style=\"min-width:320px;max-width:1920px;width:100%;color:red;background-color:white;border-top:1px solid darkred; border-bottom:1px solid darkred;\">" + ExceptionPrefix + error);
    }
}
