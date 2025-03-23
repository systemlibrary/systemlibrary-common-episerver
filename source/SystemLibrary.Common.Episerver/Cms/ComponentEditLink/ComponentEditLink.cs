using EPiServer.Core;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Episerver;

internal static class ComponentEditLink
{
    public static string Create(IContent content, int level)
    {
        if (content?.ContentLink == null) return null;

        // Performance: getting the identity this way as most scenarios do not have a auth'd user, so it will "fail fast"
        var httpContext = HttpContextInstance.Current;
        var identity = httpContext?.User?.Identity;

        if (identity?.IsAuthenticated != true) return null;

        if (httpContext.Request?.QueryString.Value?.Contains("epieditmode=") == true) return null;

        var currentUser = new AppCurrentUser();

        if (currentUser.IsCmsUser)
        {
            var language = Services.Get<IContentLanguageAccessor>();
            var lang = language?.Language?.TwoLetterISOLanguageName;
            if(level == 0)
                return "<a href='/episerver/cms#context=epi.cms.contentdata:///" + content.ContentLink.ID + "&viewsetting=viewlanguage:///" + lang + "' style='position:relative!important;float:right!important;background:#0037FF!important;color:white;z-index:99999!important;border-radius:6px;display:flex;flex: 0 0 21px !important;margin-left:auto;align-items:center;justify-content:center;text-decoration:none;height:14px;max-height:14px!important;max-width:21px!important;width:21px;font-size:14px;transform:rotate(130deg);top:-6px!important;align-self:flex-end;right:8px!important;' class='sle-component-edit'>&#9999;</a>";

            return "<a href='/episerver/cms#context=epi.cms.contentdata:///" + content.ContentLink.ID + "&viewsetting=viewlanguage:///" + lang + "' style='position:relative!important;float:right!important;background:#337AFF!important;color:white;z-index:99999!important;border-radius:6px;display:flex;flex: 0 0 21px !important;margin-left:auto;align-items:center;justify-content:center;text-decoration:none;height:12px;max-height:12px!important;max-width:19px!important;width:19px;font-size:12px;transform:rotate(130deg);top:-6px!important;align-self:flex-end;right:8px!important;' class='sle-component-edit'>&#9999;</a>";
        }
        return null;
    }
}
