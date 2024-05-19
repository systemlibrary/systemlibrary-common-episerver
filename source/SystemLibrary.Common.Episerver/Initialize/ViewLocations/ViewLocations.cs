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
        "/Pages/{1}/{0}.cshtml",
        "/Pages/{1}/Views/{0}.cshtml",
        "/Pages/Setting/{1}/{0}.cshtml",
        "/Pages/Singleton/{1}/{0}.cshtml",

        "/Content/Pages/{1}/{0}.cshtml",
        "/Content/Pages/{1}/Views/{0}.cshtml",
        "/Content/Pages/Setting/{1}/{0}.cshtml",
        "/Content/Pages/Singleton/{1}/{0}.cshtml"
    };

    // Blocks are obsolete in >= .NET 6, as "ViewComponents" are preferred way of creating "BlockControllers"
    // and ViewComponents in >= .NET 6, have a hardcoded "Components/" path as prefix:
    // https://github.com/aspnet/Mvc/blob/e985fa5d423968f36a651a354d9c953192c52532/src/Microsoft.AspNet.Mvc.ViewFeatures/ViewComponents/ViewViewComponentResult.cs
    internal static string[] GetViewsForBlocks() => new string[]
    {
        "/Components/{0}/Index.cshtml",
        "/Components/{1}/{0}.cshtml",
        "/Components/Content/{0}/Index.cshtml",
        "/Components/Content/{1}/{0}.cshtml",
        "/Components/Container/{0}/Index.cshtml",
        "/Components/Container/{1}/{0}.cshtml",
        "/Components/Element/{0}/Index.cshtml",
        "/Components/Element/{1}/{0}.cshtml",
        "/Components/Singleton/{0}/Index.cshtml",
        "/Components/Singleton/{1}/{0}.cshtml",

        "/Content/Components/{0}/Index.cshtml",
        "/Content/Components/{1}/{0}.cshtml",
        "/Content/Components/Content/{0}/Index.cshtml",
        "/Content/Components/Content/{1}/{0}.cshtml",
        "/Content/Components/Container/{0}/Index.cshtml",
        "/Content/Components/Container/{1}/{0}.cshtml",
        "/Content/Components/Element/{0}/Index.cshtml",
        "/Content/Components/Element/{1}/{0}.cshtml",
        "/Content/Components/Singleton/{0}/Index.cshtml",
        "/Content/Components/Singleton/{1}/{0}.cshtml",
    };
    
    static string[] _AllViews;

    internal static string[] AllViews = (_AllViews != null) ? _AllViews :
        (_AllViews = GetViewsForPages()
        .Concat(GetViewsForBlocks())
        .Concat(GetViews()).ToArray());
}
