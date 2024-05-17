﻿using System.Security.Principal;

using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;

using Microsoft.AspNetCore.Identity;

namespace SystemLibrary.Common.Episerver;

internal static class Globals
{
    // Last part 'Cms' refers to the FolderName in root of this project and it matches any controller at any depth below it
    internal const string AreaCms = "/SystemLibrary/CommonEpiserverCms";
    internal const string AreaFontAwesome = "/SystemLibrary/CommonEpiserverFontAwesome";

    internal static bool IsDeveloping => true;

    internal static bool IsUnitTesting = false;

    internal static string LibraryBasePath
    {
        get
        {
            return "C:\\syslib\\systemlibrary-common-episerver\\source\\SystemLibrary.Common.Episerver\\";
        }
    }

    internal static Type ContentDataType = typeof(ContentData);
    internal static Type IPrincipalType = typeof(IPrincipal);
    internal static Type IdentityUserType = typeof(IdentityUser);
}
