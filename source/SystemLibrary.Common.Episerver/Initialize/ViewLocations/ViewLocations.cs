using System.Linq;

namespace SystemLibrary.Common.Episerver.Initialize;

internal class ViewLocations
{
    static string[] GetViews() => new string[]
    {
        // Required by Episerver
        "/Views/Shared/{0}.cshtml", 
    };

    static string[] GetViewsForPages() => new string[]
    {
        // View locations for Pages
        "/Pages/{1}/{0}.cshtml",
        "/Pages/{1}/Views/{0}.cshtml",
        "/Pages/Singleton/{1}/{0}.cshtml",
        "/Pages/Content/{1}/{0}.cshtml",
        "/Pages/Container/{1}/{0}.cshtml",
        "/Pages/Setting/{1}/{0}.cshtml",
        "/Content/Pages/{1}/{0}.cshtml",
        "/Content/Pages/{1}/Views/{0}.cshtml",
        "/Content/Pages/Singleton/{1}/{0}.cshtml",
        "/Content/Pages/Container/{1}/{0}.cshtml",
        "/Content/Pages/Content/{1}/{0}.cshtml",
        "/Content/Pages/Setting/{1}/{0}.cshtml"
    };

    // Blocks are empty in .NET 7, as "ViewComponents" are preferred way of creating "BlockControllers"
    // and ViewComponents in >= .NET 6, has a hardcoded "Components/" path as prefix:
    // https://github.com/aspnet/Mvc/blob/e985fa5d423968f36a651a354d9c953192c52532/src/Microsoft.AspNet.Mvc.ViewFeatures/ViewComponents/ViewViewComponentResult.cs
    static string[] GetViewsForBlocks() => new string[]
    {
        "/Content/Components/{1}/{0}.cshtml",
        "/Content/Blocks/{1}/{0}.cshtml",
        "/Content/Blocks/{1}/Views/{0}.cshtml",
    };
    
    static string[] _AllViews;

    internal static string[] AllViews = (_AllViews != null) ? _AllViews :
        (_AllViews = GetViewsForPages()
        .Concat(GetViewsForBlocks())
        .Concat(GetViews()).ToArray());
}
