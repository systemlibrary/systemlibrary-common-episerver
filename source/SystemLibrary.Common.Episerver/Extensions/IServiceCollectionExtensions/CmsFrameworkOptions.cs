using SystemLibrary.Common.Framework;

namespace SystemLibrary.Common.Episerver.Extensions;

public class CmsFrameworkOptions : FrameworkOptions
{
    /// <summary>
    /// Set the minimum password length for the default CMS DB login
    /// <para>Defaults to 12</para>
    /// <para>Passwords do not require a digit nor a special char, only minimum one lower and one upper cased char in the password</para>
    /// <para>Wrong passwords attempts are 10, with a 15 minute locked out time</para>
    /// </summary>
    public int ApplicationCookieMimumPasswordLength = 12;

    /// <summary>
    /// Sets the application cookies sliding expiration
    /// <para>Defaults to true</para>
    /// <para>This is the CMS identity cookie that must exist to have access to the CMS UI, allowing up to 20 hours of inactivity and still be logged in</para>
    /// </summary>
    public bool ApplicationCookieSlidingExpiration = true;

    /// <summary>
    /// Set the application cookie duration in minutes
    /// <para>Defaults to 1200 (20 hours)</para>
    /// <para>Sliding expiration: always on</para>
    /// <para>Maximum</para>
    /// <para>This is the CMS cookie that is required to have to be granted access to the CMS UI</para>
    /// </summary>
    public int ApplicationCookieDuration = 1200;

    /// <summary>
    /// Enforce a session deletion of session if session was created more than 30 days ago
    /// <para>Set to true to enable a deletion of Application.Cookie and a redirect to startpage if session has laster for more than 30 days</para>
    /// </summary>
    public bool UseApplicationCookieMaxSessionDuration = true;

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
    public string DefaultAdminPassword = "Demo123456!!";

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
    /// Maximum upload size in bytes, defaults to 1GB
    /// </summary>
    public long UploadLimitBytes = 1073741824;

    /// <summary>
    /// Adds all react services and the V8 engine for react server side rendering
    /// <para>- Set to false if you do not use react server side</para>
    /// </summary>
    public bool AddReactServerSideServices = true;

    public bool UseExceptionHandler = true;

    /// <summary>
    /// If you want a custom login path like "SignIn", set this
    /// </summary>
    public string CmsLoginPath = null;

    /// <summary>
    /// Return a path of where to look for components view files.
    /// <para>Components that do not have a Controller nor a Component class defined, will use this path as the base path of where to look for views</para>
    /// Example: ~/Content/Components/
    /// </summary>
    public Func<Type, string> DefaultComponentPathPredicate = null;

    /// <summary>
    /// Enables loading of React Server Side through Node (V8 engine)
    /// <para>- does not load babel nor react, so your build must come with a React version and the already transpiled files for Node (basically "dist" folder)</para>
    /// Example might be:
    /// <para>~/static/dist/vendor-react.js</para>
    /// <para>~/static/dist/vendor-others.js</para>
    /// <para>~/static/dist/server-side-app.js</para>
    /// Where all these files are transpiled and minimized already before being loaded by the C# code
    /// </summary>
    public string[] ReactSsrScriptsInOrder = null;

    /// <summary>
    /// Max usages per engine when environment is 'local'
    /// </summary>
    public int ReactSsrMaxUsagesPerEngineLocal = 10;

    /// <summary>
    /// Max usages per engine before it resets when environment is not 'local'
    /// </summary>
    public int ReactSsrMaxUsagesPerEngineNonLocal = 10000;

    /// <summary>
    /// Enable the IErrorPage interface middleware
    /// <para>Implement your own ErrorPage.cs and a View, and inherit/implement IErrorPage interface</para>
    /// <para>Set then the StatusCode on the ErrorPage.cs inside Epi UI to let that page respond to for instance any 404 in your whole application</para>
    /// <para>This is never shown on path / nor if path starts with "/episerver" </para>
    /// <para>If the request sends accept "application/json" the ErrorPage you've created is not invoked, but a hard-coded json error is returned</para>
    /// </summary>
    /// <remarks>
    /// You can set it to true to enable the parts in the comment that does not involve IErrorPage, even if you do not have a single IErrorPage implementation
    /// </remarks>
    /// <example>
    /// Example: Adding a middleware that triggers after 404, and after error page response, to for instance set status to 401 unauthorized or even a 301 redirect:
    /// <code>
    /// Program.cs:
    /// app.UseCommonsCmsApp(options);  // This might return a custom 404 page as a response
    /// app.Use&gt;RedirectMiddleware&lt;();    // We override the response for a particular url, to redirect instead of giving 404
    /// 
    /// class RedirectMiddleware
    /// {
    ///     public async Task InvokeAsync(HttpContext context)
    ///     {
    ///         await next(context);
    ///          
    ///         if(context.Request.Path == "/redirect-as-url-do-not-exist") {
    ///             context.Response.Clear();       // Clear the previously set response, 404
    ///             context.Response.StatusCode = 301;
    ///             context.Response.Redirect("www.systemlibrary.com");
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public bool UseErrorPageResponse = false;

}