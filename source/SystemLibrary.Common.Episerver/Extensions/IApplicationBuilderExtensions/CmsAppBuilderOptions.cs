using System;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Contains application builder options that you can override if you want to
/// </summary>
public class CmsAppBuilderOptions : Web.Extensions.AppBuilderOptions
{
    public bool UseExceptionHandler = true;
    public bool UseMapContentAndControllers = true;
    public string CmsLoginPath = null;
    public Func<string, string> DefaultComponentPathPredicate = null;

    /// <summary>
    /// Disallow serving known application files:
    /// - everything under the path ~/config
    /// - everything under the path /bin/
    /// - appSettings.json
    /// - any .config file
    /// - any .dll file
    /// - any .cs file
    /// - any .cshtml file
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
}