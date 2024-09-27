using System.Security.Claims;

using EPiServer.Core;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver;

internal static class ComponentEditLink
{
    public static string Create(ClaimsPrincipal user, IContent content)
    {
        var identity = user?.Identity;
        if (identity != null && identity.IsAuthenticated)
        {
            if (content != null)
            {
                if (user.IsInAnyRole(Roles.AdminRoles))
                {
                    return "<a href='/episerver/cms#context=epi.cms.contentdata:///" + content.ContentLink.ID + "&viewsetting=viewlanguage:///' style='position:relative;top:0;float:right;background:#0037FF;color:white;z-index:99999;border-radius:6px;display:flex;align-items:center;justify-content:center;text-decoration:none;height:15px;width:24px;font-size:16px;transform:rotate(143deg);top:-2px;right:-8px;' class='sle-component-edit'>&#9999;</a>";
                }
            }
        }
        return null;
    }
}
