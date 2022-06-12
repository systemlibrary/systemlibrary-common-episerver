
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
    ///         "iLogWriter": {
    ///             "appendLoggedInState": false,
    ///             "appendCurrentPage": false,
    ///             "appendCurrentUrl": false,
    ///             "appendIp": false,
    ///             "appendBrowser": false,
    ///             "appendCookieInfo": false
    ///        }
    ///      }
    ///     ...
    /// }
    /// </code>
    /// </example>
    internal class AppSettingsConfig : Config<AppSettingsConfig>
    {
        public AppSettingsConfig()
        {
            SystemLibraryCommonEpiserver = new Configuration();
        }

        public class Configuration
        {
            public Configuration()
            {
                ILogWriter = new ILogWriterConfiguration();
            }

            public class ILogWriterConfiguration
            {
                public bool AppendLoggedInState { get; set; } = true;
                public bool AppendCurrentPage { get; set; } = true;
                public bool AppendCurrentUrl { get; set; } = true;
                public bool AppendIp { get; set; } = false;
                public bool AppendBrowser { get; set; } = false;
                public bool AppendCookieInfo { get; set; } = false;
            }
            public ILogWriterConfiguration ILogWriter { get; set; }
        }

        public Configuration SystemLibraryCommonEpiserver { get; set; }
    }
}
