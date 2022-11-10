using SystemLibrary.Common.Episerver.Cms.Properties;
using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver;

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
///         "enabled": {
///             "enabled": true,
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
public class AppSettings : Config<AppSettings>
{
    public Configuration SystemLibraryCommonEpiserver { get; set; }

    public ConnectionStringsConfiguration ConnectionStrings { get; set; }
    public EditConfiguration Edit => SystemLibraryCommonEpiserver.Edit;

    public AppSettings()
    {
        SystemLibraryCommonEpiserver = new Configuration();
        ConnectionStrings = new ConnectionStringsConfiguration();
    }
}

public class Configuration
{
    public Configuration()
    {
        Edit = new EditConfiguration();
    }

    public EditConfiguration Edit { get; set; }
}

public class ConnectionStringsConfiguration
{
    public string EPiServerDB { get; set; }
}

public class EditConfiguration
{
    public bool HideLanguageColumnInVersionGadget { get; set; } = false;
    public string ContentCreationBackgroundColor { get; set; } = "";
    public string ContentCreationBorderColor { get; set; } = "";
    public string PageTreeSelectedContentBorderColor { get; set; } = "";
    public string ContentTitleColor { get; set; } = "";
    public string ActiveProjectBarBackgroundColor { get; set; } = "red";

    public PropertiesConfiguration Properties { get; set; }

    public EditConfiguration()
    {
        Properties = new PropertiesConfiguration();
    }
}

public class PropertiesConfiguration
{
    public MessageConfiguration Message { get; set; }
    public PropertiesConfiguration()
    {
        Message = new MessageConfiguration();
    }
}
