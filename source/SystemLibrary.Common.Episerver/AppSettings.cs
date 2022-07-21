using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// Override default configurations in 'SystemLibrary.Common.Episerver' by adding 'systemLibraryCommonEpiserver' object to 'appSettings.json'
    /// </summary>
    /// <example>
    /// 'appSettings.json'
    /// <code class="language-csharp hljs">
    /// {
    ///     ...,
    ///     "systemLibraryCommonEpiserver": {
    ///         "logMessageBuilderOptions": {
    ///             "appendLoggedInState": false,
    ///             "appendCurrentUrl": false,
    ///             "appendIp": false,
    ///             "appendBrowser": false,
    ///             "appendCookieInfo": false
    ///        },
    ///         "editMode": {
    ///             "hideLanguageColumnInVersionGadget": true,
    ///             "contentCreationBackgroundColor": "#B84D94",
    ///             "contentCreationBorderColor": "#B84D94",
    ///             "pageTreeSelectedContentBorderColor": "#B84D94",
    ///             "contentTitleColor": "#B84D94",
    ///             "activeProjectBarBackgroundColor": "#B84D94"
    ///         }
    ///      }
    ///     ...
    /// }
    /// </code>
    /// </example>
    internal class AppSettings : Config<AppSettings>
    {
        public AppSettings()
        {
            SystemLibraryCommonEpiserver = new Configuration();
            ConnectionStrings = new ConnectionStringsConfiguration();
        }

        public class ConnectionStringsConfiguration
        {
            public string EPiServerDB { get; set; }
        }

        public class Configuration
        {
            public Configuration()
            {
                CmsEdit = new CmsEditConfiguration();
            }

            public class CmsEditConfiguration
            {
                public bool HideLanguageColumnInVersionGadget { get; set; } = false;
                public string ContentCreationBackgroundColor { get; set; } = "";
                public string ContentCreationBorderColor { get; set; } = "";
                public string PageTreeSelectedContentBorderColor { get; set; } = "";
                public string ContentTitleColor { get; set; } = "";
                public string ActiveProjectBarBackgroundColor { get; set; } = "";
            }
           
            public CmsEditConfiguration CmsEdit { get; set; }
        }

        public ConnectionStringsConfiguration ConnectionStrings { get; set; }
        public Configuration SystemLibraryCommonEpiserver { get; set; }
    }
}
