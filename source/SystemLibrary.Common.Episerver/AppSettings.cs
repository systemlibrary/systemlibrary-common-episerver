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
    ///         "logMessageBuilder": {
    ///             "appendLoggedInState": false,
    ///             "appendCurrentPage": false,
    ///             "appendCurrentUrl": false,
    ///             "appendIp": false,
    ///             "appendBrowser": false,
    ///             "appendCookieInfo": false
    ///        },
    ///        "cache": {
    ///             "defaultDuration": 180
    ///         },
    ///         "editMode": {
    ///             "companyColor": "#B84D94" //Can be a css color or a css hex color
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
                LogMessageBuilder = new LogMessageBuilderOptions();
                Cache = new CacheConfiguration();
                EditMode = new EditModeConfiguration();
            }

            public class EditModeConfiguration
            {
                public string CompanyColor { get; set; }
            }

            public class LogMessageBuilderOptions
            {
                public bool AppendLoggedInState { get; set; } = true;
                public bool AppendCurrentPage { get; set; } = true;
                public bool AppendCurrentUrl { get; set; } = true;
                public bool AppendIp { get; set; } = false;
                public bool AppendBrowser { get; set; } = false;
                public bool AppendCookieInfo { get; set; } = false;
            }

            public class CacheConfiguration
            {
                public int DefaultDuration { get; set; } = 180;
            }

            public EditModeConfiguration EditMode { get; set; }
            public CacheConfiguration Cache { get; set; }
            public LogMessageBuilderOptions LogMessageBuilder { get; set; }
        }

        public ConnectionStringsConfiguration ConnectionStrings { get; set; }
        public Configuration SystemLibraryCommonEpiserver { get; set; }
    }
}
