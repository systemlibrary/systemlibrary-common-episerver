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
    /// 
    /// Components that do not have a Controller nor a Component class defined, will use this path as the base path of where to look for views
    /// 
    /// Example: ~/Blocks/
    /// </summary>
    public Func<Type, string> DefaultComponentPathPredicate = null;

    /// <summary>
    /// Disallow serving known application files:
    /// - everything under the path ~/config
    /// - everything under the path /bin/
    /// - appSettings.json
    /// - any .config file
    /// - any .dll file
    /// - any .cs file
    /// - any .cshtml file
    /// - any .tsx file
    /// - any .ts file
    /// - any .mdf file
    /// - any .sql file
    /// - any .db file
    /// 
    /// Note: If set to True, this stops the pipeline, and this is registered before StaticFileHandler
    /// </summary>
    public bool DisallowKnownAppFiles = true;

    /// <summary>
    /// Enables loading of React Server Side through Node (V8 engine)
    /// - does not load babel nor react, so your build must come with React and transpilation for Node
    /// 
    /// Example might be:
    /// ~/static/dist/vendor-react.js
    /// ~/static/dist/vendor-others.js
    /// ~/static/dist/server-side-app.js
    /// </summary>
    public string[] ReactSsrScriptsInOrder;

    /// <summary>
    /// Set to true to return a 404 response with proper content based on response content-type header
    /// - a response of xml returns a standard xml error response
    /// - a response of json returns a standard json error response
    /// - a response of an image file it returns a 
    /// 
    /// NOTE: If a path to CMS page is not found, implement IErrorPage to a page type which will be used
    /// NOTE: If IErrorPage is not implemented, this does nothing for CMS pages not found
    /// 
    /// </summary>
    public bool UseErrorPageResponse;
}