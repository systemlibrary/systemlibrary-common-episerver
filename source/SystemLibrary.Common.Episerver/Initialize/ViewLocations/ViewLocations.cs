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
        "~/Content/Pages/{1}/Views/{0}.cshtml",
        "~/Content/Pages/Singleton/{1}/{0}.cshtml",
        "~/Content/Pages/Content/{1}/{0}.cshtml",
        "~/Content/Pages/Setting/{1}/{0}.cshtml",
    };

    static string[] GetViewsForBlocks() => new string[] 
    {
        "~/Content/Blocks/{0}/Index.cshtml",
        "~/Content/Blocks/{1}/{0}.cshtml",

        "~/Content/Blocks/Container/{0}/Index.cshtml",
        "~/Content/Blocks/Container/{1}/{0}.cshtml",

        "~/Content/Blocks/Content/{0}/Index.cshtml",
        "~/Content/Blocks/Content/{1}/{0}.cshtml",

        "~/Content/Blocks/Singleton/{0}/Index.cshtml",
        "~/Content/Blocks/Singleton/{1}/{0}.cshtml"
    };

    static string[] _AllViews;

    internal static string[] AllViews = (_AllViews != null) ? _AllViews :
        (_AllViews = GetViewsForPages()
        .Concat(GetViewsForBlocks())
        .Concat(GetViews()).ToArray());
}
