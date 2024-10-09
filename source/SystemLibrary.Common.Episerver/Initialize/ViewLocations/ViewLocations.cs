namespace SystemLibrary.Common.Episerver.Initialize;

internal class ViewLocations
{
    static string[] GetViews() => new string[]
    {
        "/Views/{0}.cshtml",
        "/Views/Shared/{0}.cshtml", // Required by Episerver
    };

    static string[] GetViewsForPages() => new string[]
    {
         "/Pages/{1}/{0}.cshtml",
         "/Pages/{1}/Views/{0}.cshtml",
         "/Pages/Settings/{1}/{0}.cshtml",
         "/Pages/Singletons/{1}/{0}.cshtml",
         "/Pages/SPA/{1}/{0}.cshtml",

         "/Content/Pages/{1}/{0}.cshtml",
         "/Content/Pages/{1}/Views/{0}.cshtml",
         "/Content/Pages/Settings/{1}/{0}.cshtml",
         "/Content/Pages/Singletons/{1}/{0}.cshtml",
         "/Content/Pages/SPA/{1}/{0}.cshtml",
    };

    static string[] GetViewsForComponents() => new string[]
    {
        "/{0}.cshtml",
        "/Content/{0}.cshtml",
        "/Components/{0}.cshtml",
    };

    // NOTE: Blocks are obsolete in >= Epi 12, using Components from "here on"
    // Side note: ViewComponents in >= .NET 6 have a hardcoded "Components/" path as prefix:
    // So all 
    // https://github.com/aspnet/Mvc/blob/e985fa5d423968f36a651a354d9c953192c52532/src/Microsoft.AspNet.Mvc.ViewFeatures/ViewComponents/ViewViewComponentResult.cs
    internal static string[] GetViewsForBlocks() => new string[]
    {
        "/Content/Blocks/{1}/{0}.cshtml",
    };

    static string[] _AllViews;

    internal static string[] AllViews = _AllViews ??= GetViews()
        .Concat(GetViewsForComponents())
        .Concat(GetViewsForPages())
        .Concat(GetViewsForBlocks()).ToArray();
}
