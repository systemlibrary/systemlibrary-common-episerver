using System.Security.Claims;

using EPiServer.Core;

using SystemLibrary.Common.Episerver.Users;

namespace SystemLibrary.Common.Episerver;

internal static class ComponentEditLink
{
    public static string Create(ClaimsPrincipal user, IContent content)
    {
        if (content?.ContentLink == null) return null;

        var identity = user?.Identity;

        if (identity?.IsAuthenticated != true) return null;
            
        var currentUser = new CurrentUser();

        if (currentUser.IsCmsUser)
        {
            return "<a href='/episerver/cms#context=epi.cms.contentdata:///" + content.ContentLink.ID + "&viewsetting=viewlanguage:///' style='position:relative;float:right;background:#0037FF;color:white;z-index:99999;border-radius:6px;display:flex;align-items:center;justify-content:center;text-decoration:none;height:14px;width:21px;font-size:14px;transform:rotate(130deg);top:-4px;right:-8px;' class='sle-component-edit'>&#9999;</a>";
        }
        return null;
    }
}
