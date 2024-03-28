using JavaScriptEngineSwitcher.V8;

namespace SystemLibrary.Common.Episerver.Extensions;

public class CommonCmsServicesOptions : Web.Extensions.CommonWebApplicationServicesOptions
{
    public bool AddDisplays = true;

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
    /// Registered the 'ApplicationCookie' to be the login cookie in the CMS
    /// - CmsUsersSlidingExpiration handled the sliding expiartion
    /// - CmsUsersSignedInDurationMinutes handles cookie duration
    /// </summary>
    public bool ConfigureApplicationCookie = true;

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

    /// <summary>
    /// Maximum upload size in bytes, defaults to 30MB
    /// </summary>
    public long UploadLimitBytes = 30720000;

    /// <summary>
    /// Set to True when your DB is empty and brand new, to initialize the DB with language, an admin user and site settings
    /// - Set to False afterwards 
    ///
    /// NOTE: From Epi 12.26.0 to 12.26.1 they broke the way this is done, if Env.WebRootPath also is set, then this initializer is returning 500 "as it is the first request"
    /// </summary>
    public bool RunFirstRequestInitializer = true;
}