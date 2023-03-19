using System;
using System.Linq.Expressions;

using EPiServer.Web.Mvc.Html;

using Microsoft.AspNetCore.Html;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class HtmlHelperExtensions
{
    public static IHtmlContent PropertyForContentArea<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
        return html.PropertyFor(expression, new { SkipWrapperTag = true, HasContainer = false, HasItemContainer = false });
    }
}
