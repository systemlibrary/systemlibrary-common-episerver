using System.Linq;

namespace SystemLibrary.Common.Episerver.Initialize;

internal class ViewLocations
{
    static string[] GetViews() => new string[] 
    {
        "~/Views/Shared/{0}.cshtml", //Required by Episerver
    };

    static string[] GetViewsForPages() => new string[] 
    {
        "~/Content/Pages/{1}/{0}.cshtml",
        "~/Content/Pages/{1}/Views/{0}.cshtml"
    };

    static string[] GetViewsForBlocks() => new string[] 
    {
        "~/Content/Blocks/{0}/Index.cshtml",
        "~/Content/Blocks/{1}/{0}.cshtml"
    };

    static string[] GetViewsForComponents() => new string[]
    {
         "~/Content/Components/{0}.cshtml",
         "~/Content/Components/{0}/Index.cshtml"
    };

    static string[] _AllViews;

    internal static string[] AllViews = (_AllViews != null) ? _AllViews :
        (_AllViews = GetViewsForComponents()
        .Concat(GetViewsForPages())
        .Concat(GetViewsForBlocks())
        .Concat(GetViews()).ToArray());
}
