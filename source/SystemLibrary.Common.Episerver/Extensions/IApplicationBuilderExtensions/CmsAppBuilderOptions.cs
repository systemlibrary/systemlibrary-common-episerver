namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Contains application builder options that you can override if you want to
/// </summary>
public class CmsAppBuilderOptions : Web.Extensions.AppBuilderOptions
{
    public bool UseExceptionHandler = true;

    public bool UseMapContentAndControllers = true;

    /// <summary>
    /// If you want a custom login path like "SignIn", set this
    /// </summary>
    public string CmsLoginPath = null;

    /// <summary>
    /// Return a path of where to look for components view files.
    /// <para>Components that do not have a Controller nor a Component class defined, will use this path as the base path of where to look for views</para>
    /// Example: ~/Blocks/
    /// </summary>
    public Func<Type, string> DefaultComponentPathPredicate = null;

    /// <summary>
    /// Disallow serving known application files:
    /// <list>
    /// <item>- everything under the path ~/config</item>
    /// <item>- everything under the path /bin/</item>
    /// <item>- appSettings.json</item>
    /// <item>- any .config filev</item>
    /// <item>- any .dll file</item>
    /// <item>- any .cs file</item>
    /// <item>- any .cshtml file</item>
    /// <item>- any .tsx file</item>
    /// <item>- any .ts file</item>
    /// <item>- any .mdf file</item>
    /// <item>- any .sql file</item>
    /// <item>- any .db file</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// If set to True, this stops the pipeline and returns blank, even before StaticFileHandler, so if you need to host any of these file types register your own static file handler for those file types before or disable this one
    /// </remarks>
    public bool DisallowKnownAppFiles = true;

    /// <summary>
    /// Enables loading of React Server Side through Node (V8 engine)
    /// <para>- does not load babel nor react, so your build must come with a React version and the already transpiled files for Node (basically "dist" folder)</para>
    /// Example might be:
    /// <list>
    /// <item>~/static/dist/vendor-react.js</item>
    /// <item>~/static/dist/vendor-others.js</item>
    /// <item>~/static/dist/server-side-app.js</item>
    /// </list>
    /// Where all these files are transpiled and minimized already before being loaded by the C# code
    /// </summary>
    public string[] ReactSsrScriptsInOrder;

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
    public bool UseErrorPageResponse;
}