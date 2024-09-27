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
            return "<a href='/episerver/cms#context=epi.cms.contentdata:///" + content.ContentLink.ID + "&viewsetting=viewlanguage:///' style='position:relative!important;float:right!important;background:#0037FF!important;color:white;z-index:99999!important;border-radius:6px;display:flex;flex: 0 0 21px !important;margin-left:auto;align-items:center;justify-content:center;text-decoration:none;height:14px!important;width:21px!important;font-size:14px;transform:rotate(130deg);top:-6px!important;align-self:flex-end;right:8px!important;' class='sle-component-edit'>&#9999;</a>";
        }
        return null;
    }
}
