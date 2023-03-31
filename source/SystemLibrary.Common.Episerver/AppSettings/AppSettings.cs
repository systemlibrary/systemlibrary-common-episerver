using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Override default configurations in 'SystemLibrary.Common.Episerver' by adding 'systemLibraryCommonEpiserver' object to 'appSettings.json'
/// </summary>
/// <example>
/// 'appSettings.json'
/// <code class="language-csharp hljs">
/// {
///    ...,
///    "systemLibraryCommonEpiserver": {
///        "edit": {
///          "showEditStyle": false,
///          "showEditFieldsAsColumns": false,
///          "hideLanguageColumnInVersionGadget": false,
///          "contentCreationBackgroundColor": "",
///          "contentCreationBorderColor": "",
///          "pageTreeSelectedContentBorderColor": "",
///          "contentTitleColor": "",
///          "activeProjectBarBackgroundColor": ""
///        },
///        "properties": {
///           "message": {
///              "backgroundColor": "#ededed",
///              "textColor": "#080736"
///           }
///        }
///     }
///     ...
/// }
/// </code>
/// </example>
public partial class AppSettings : Config<AppSettings>
{
    public ConnectionStringsConfiguration ConnectionStrings { get; set; }
    public Configuration SystemLibraryCommonEpiserver { get; set; }
    
    internal EditConfiguration Edit => SystemLibraryCommonEpiserver.Edit;
    internal PropertiesConfiguration Properties => SystemLibraryCommonEpiserver.Properties;

    public AppSettings()
    {
        SystemLibraryCommonEpiserver = new Configuration();
        ConnectionStrings = new ConnectionStringsConfiguration();
    }

    public class Configuration
    {
        public Configuration()
        {
            Edit = new EditConfiguration();
            Properties = new PropertiesConfiguration();
        }

        public EditConfiguration Edit { get; set; }

        public PropertiesConfiguration Properties { get; set; }
    }
}
