namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// This class has a good example on how to setup 'SystemLibrary.Common.Episerver' in your application
    /// 
    /// Click the 'Examples' button to view all the info you need to get going
    /// </summary>
    /// <example>
    /// Setup XML and JSON configurations:
    /// 
    /// // Create or modify ~/module.config at root, to load common CSS stylesheets when in Edit Mode
    /// <code class="language-csharp hljs">
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;module&gt;
    ///     &lt;clientResources&gt;
    ///         &lt;add name = "epi-cms.widgets.base" path="/SystemLibrary/ContentIconAttribute/FontAwesome" resourceType="Style"/&gt;
    ///     &lt;/clientResources>
    /// &lt;/module&gt;
    /// &lt;/example&gt;
    /// </code>
    /// 
    /// // Create or modify ~/appSettings.json, to target a EpiDb as normal, but can also configure settings for the SystemLibrary packages
    /// <code class="language-csharp hljs">
    /// {
    ///     "ConnectionStrings": {
    ///         "EPiServerDB": "Data Source=.\\sqlexpress;Initial Catalog=EPiServerDb;Connection Timeout=10;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=true"
    ///     },
    ///     "EPiServer": {
    ///         "Find": {
    ///             ...
    ///         }
    ///     },
    ///     
    ///     ...
    ///     
    ///     "systemLibraryCommonNet": {
    ///         "dump": {
    ///             "folder": "C:\\Logs",
    ///             "fileName": "output.log"
    ///         }
    ///     },
    ///     "systemLibraryCommonNetJson": {
    ///         "maxDepth": 64,
    ///         "allowTrailingCommas": true,
    ///         "propertyNameCaseInsensitive": true,
    ///         "writeIndented": false
    ///     },
    ///     "systemLibraryCommonWebHttpBaseClient": {
    ///         "retryRequestTimeoutSeconds": 30000,
    ///         "cacheClientConnectionInSeconds": 300
    ///     },
    ///     "systemLibraryCommonEpiserver": {
    ///         "iLogWriter": {
    ///             "appendLoggedInState": false,
    ///             "appendCurrentPage": false,
    ///             "appendCurrentUrl": false,
    ///             "appendIp": false,
    ///             "appendBrowser": false,
    ///             "appendCookieInfo": false
    ///         }
    ///     }
    ///     ...
    ///}
    ///</code>
    ///
    /// // Configure static Main()
    ///<code class="language-csharp hljs">
    /// //Custom path to appSettings.json, or place it at root
    /// var appSettingsPath = AppContext.BaseDirectory + "\\Configurations\\AppSettings\\appSettings.json";
    /// Host.CreateDefaultBuilder(args)
    ///     .ConfigureAppConfiguration(config => config.AddJsonFile(appSettingsPath))
    ///     .ConfigureWebHostDefaults(config => {
    ///     config.UseEnvironment("Release");
    ///     config.UseStartup&lt;Initialize&gt;();
    ///     })
    ///     .ConfigureCmsDefaults()
    ///     .Build()
    ///     .Run();
    /// </code>
    /// 
    /// // Configure 'Initialize' class registered in Main() above
    /// 
    /// <code class="language-csharp hljs">
    /// public class Initialize
    /// {
    ///     public void ConfigureServices(IServiceCollection services)
    ///     {
    ///         //options is 'null' in the sample, you can new it up or remove it
    ///         services.CommonEpiserverServices&lt;CurrentUser&gt;(options);
    ///         services.AddCms();
    ///         services.AddTinyMce();
    ///     }
    ///
    ///     public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    ///     {
    ///         //options is 'null' in the sample, you can new it up or remove it
    ///         app.CommonEpiserverAppBuilder(options);
    ///     }
    /// }
    /// </code>
    ///</example>
    public static class Setup
{
}
}
