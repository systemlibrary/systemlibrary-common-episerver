using System;

using JavaScriptEngineSwitcher.V8;

namespace SystemLibrary.Common.Episerver.Extensions;

public class CommonCmsServicesOptions : Web.Extensions.CommonWebApplicationServicesOptions
{
    /// <summary>
    /// Add mobile, tablet and desktop resolutions for the CMS in Edit
    /// </summary>
    public bool AddDisplays = true;

    /// <summary>
    /// Registered the 'ApplicationCookie' to be the login cookie in the CMS
    /// - CmsUsersSlidingExpiration handled the sliding expiartion
    /// - CmsUsersSignedInDurationMinutes handles cookie duration
    /// </summary>
    public bool AddApplicationCookie = true;

    /// <summary>
    /// Configure a custom login path
    /// </summary>
    /// <example>
    /// Example: "hello/world", then www.systemlibrary.com/hello/world would the CMS login screen
    /// </example>
    public string CmsLoginPath = null;

    /// <summary>
    /// Note: required ConfigureApplicationCookie to be set to True
    /// </summary>
    public bool CmsUsersSlidingExpiration = true;

    /// <summary>
    /// Note: required ConfigureApplicationCookie to be set to True
    /// </summary>
    public int CmsUsersSignedInDurationMinutes = 120;


    /// <summary>
    /// The default email of the default admin user
    /// - Only used if the DB is empty
    /// 
    /// Note: assuming the default value, 'demo' is used as username, the text before @
    /// </summary>
    public string DefaultAdminEmail = "demo@systemlibrary.com";

    /// <summary>
    /// The default password of the default admin user
    /// - Only used if the DB is empty
    /// </summary>
    public string DefaultAdminPassword = "Demo123!";

    /// <summary>
    /// A comma separated list of languageId's that comes with Episerver
    /// 
    /// Set to null or blank, if you do not want to do anything with languages, so the default that you were used to (15 default languages added, while sv and en is enabled, will then be the result)
    /// 
    /// Example of language id's:
    /// sv, en, da, fr, de, fi, no, nn, nl-BE, nl, en-AU, it-IT
    /// </summary>
    public string InitialLanguagesEnabled = "sv,en";

    public bool HideSuggestedContentTypes = true;
    public bool HidePropertyCategoryList = true;

    public bool WebSocketEnabled = true;

    public bool AutoPublishMediaOnUpload = true;

    /// <summary>
    /// Maximum upload size in bytes, defaults to 30MB
    /// </summary>
    public long UploadLimitBytes = 30720000;
}