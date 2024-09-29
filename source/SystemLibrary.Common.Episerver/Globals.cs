using System.Security.Principal;

using EPiServer.Core;

using Microsoft.AspNetCore.Identity;

namespace SystemLibrary.Common.Episerver;

internal static class Globals
{
    // Last part 'Cms' refers to the FolderName in root of this project and it matches any controller at any depth below it
    internal const string AreaCms = "/SystemLibrary/CommonEpiserverCms";
    internal const string AreaFontAwesome = "/SystemLibrary/CommonEpiserverFontAwesome";

    internal static bool IsDeveloping = false;

    internal static bool IsUnitTesting = false;

    internal static string LibraryBasePath = "C:\\syslib\\systemlibrary-common-episerver\\source\\SystemLibrary.Common.Episerver\\";
    
    internal static Type ContentDataType = typeof(ContentData);
    internal static Type IPrincipalType = typeof(IPrincipal);
    internal static Type IdentityUserType = typeof(IdentityUser);
    internal static Type ContentReferenceType = typeof(ContentReference);
    internal static Type MediaDataType = typeof(MediaData);

    internal static class CssClassName
    {
        internal const string SsrError = "sle-ssr-error";
        internal const string ViewExceptionError = "sle-view-error";
        internal const string HtmlErrorResponse = "sle-html-error";
    }
}
