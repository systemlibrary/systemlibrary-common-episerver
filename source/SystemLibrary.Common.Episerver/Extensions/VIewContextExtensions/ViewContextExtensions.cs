using EPiServer.Core;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ViewContextExtensions
{
    /// <summary>
    /// Returns true if current view context is inside a block editing view, else falseQ
    /// 
    /// //Credit to: https://marisks.net/2016/07/21/simple-check-if-block-is-in-edit-mode/
    /// 
    /// //TODO: Update to remove the need of viewContext as input param
    /// </summary>
    public static bool BlockIsInEditMode(this ViewContext viewContext)
    {
        //Credit to: https://marisks.net/2016/07/21/simple-check-if-block-is-in-edit-mode/
        var routeData = viewContext?.RouteData;
        if (routeData == null)
            routeData = HttpContextInstance.Current?.GetRouteData();

        if (routeData == null) return false;
        
        var controller = routeData.Values["pageController"] ?? routeData.Values["controller"];

        if (controller == null) return false;

        var currentContent = (routeData.Values["currentContent"] as IContent)?.ContentLink;
        var currentContentId = currentContent?.ID ?? 0;

        var pageReference = routeData.DataTokens["node"] as ContentReference;

        var pageId = pageReference?.ID ?? 0;

        return currentContentId == pageId &&
            controller.ToString() == "BlockPreview";
    }
}
