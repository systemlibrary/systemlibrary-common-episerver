namespace SystemLibrary.Common.Episerver.Extensions;

public class CmsServicesCollectionOptions : Web.Extensions.ServicesCollectionOptions
{
    /// <summary>
    /// Add mobile, tablet and desktop resolutions for the CMS in Edit
    /// </summary>
    public bool AddDisplays = true;

    /// <summary>
    /// Register the 'ApplicationCookie' to be the login cookie in the CMS
    /// <para>- Registered a service for cookie scheme "Cookies" to be used as a seperate login if wanted</para>
    /// <para>- UsersSlidingExpiration handles the sliding expiration duration</para>
    /// - UsersSignedInDuration handles cookie duration, default 180 min
    /// <para>- Sets sameSite as 'strict'</para>
    /// </summary>
    public bool AddApplicationCookies = true;

    /// <summary>
    /// Minimum CMS users password length
    /// <para>Required the AddApplicationCookie to be true </para>
    /// </summary>
    public int CmsUsersMinimumPasswordLength = 16;

    /// <summary>
    /// Note: requires ConfigureApplicationCookie to be set to True
    /// <para>- Sets both Cookies and Identity.Application scheme's sliding expiration flag</para>
    /// </summary>
    public bool UsersSlidingExpiration = false;

    /// <summary>
    /// Note: requires ConfigureApplicationCookie to be set to True
    /// <para>- Sets both Cookies and Identity.Application scheme's signed in duration in minutes</para>
    /// </summary>
    public int UsersSignedInDuration = 180;

    /// <summary>
    /// Toggle on/off the DB initialization of a new admin user, updating property order, language enabled and site definitions
    /// <para>Defaults to: false</para>
    /// Set to true if you always want 0 users in the DB your application might use an identity from Azure AD or similar
    /// </summary>
    public bool SkipInitialization = false;

    /// <summary>
    /// The default email of the default admin user
    /// <para>- Only used if the DB is empty</para>
    /// Note: assuming the default value, 'demo' is used as username, the text before @
    /// </summary>
    public string DefaultAdminEmail = "demo@systemlibrary.com";

    /// <summary>
    /// The default password of the default admin user
    /// <para>- Only used if the DB is empty</para>
    /// </summary>
    public string DefaultAdminPassword = "Demo123!";

    /// <summary>
    /// A comma separated list of languageId's that comes with Episerver
    /// <para>
    /// Set to null or blank, if you do not want to do anything with languages, so the default that you were used to (15 default languages added, while sv and en is enabled, will then be the result)
    /// </para>
    /// <para>Example of language id's:</para>
    /// sv, en, da, fr, de, fi, no, nn, nl-BE, nl, en-AU, it-IT
    /// </summary>
    public string InitialLanguagesEnabled = "sv,en";

    /// <summary>
    /// Hide the suggested content types at the top of the "New creation dialog"
    /// </summary>
    public bool HideSuggestedContentTypes = true;

    /// <summary>
    /// Hide the Category Property that all content comes with from Optimizely CMS
    /// </summary>
    public bool HidePropertyCategoryList = true;

    /// <summary>
    /// Websockets enabled by default
    /// </summary>
    public bool WebSocketEnabled = true;

    /// <summary>
    /// Auto publish on uploaded media, defaults to True
    /// </summary>
    public bool AutoPublishMediaOnUpload = true;

    /// <summary>
    /// Maximum upload size in bytes, defaults to 30MB
    /// </summary>
    public long UploadLimitBytes = 30720000;

    /// <summary>
    /// Adds all react services and the V8 engine for react server side rendering
    /// <para>- Set to false if you do not use react server side</para>
    /// </summary>
    public bool AddReactServerSideServices = true;
}