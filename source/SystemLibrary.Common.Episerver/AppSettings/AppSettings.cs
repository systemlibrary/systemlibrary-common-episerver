using SystemLibrary.Common.Net;

using static PackageConfig;

namespace SystemLibrary.Common.Episerver;

internal class AppSettings : Config<AppSettings>
{
    public AppSettings()
    {
        SystemLibraryCommonEpiserver = new PackageConfig();
        ConnectionStrings = new ConnectionStringsConfig();
    }

    public ConnectionStringsConfig ConnectionStrings { get; set; }
    public PackageConfig SystemLibraryCommonEpiserver { get; set; }

    internal EditConfig Edit => SystemLibraryCommonEpiserver.Edit;

    internal SsrConfig Ssr => SystemLibraryCommonEpiserver.Ssr;
    internal PropertiesConfig Properties => SystemLibraryCommonEpiserver.Properties;

    internal class ConnectionStringsConfig
    {
        public string EPiServerDB { get; set; }

        /// <summary>
        /// An additional optional db connection string in your appSettings.json file
        /// - If you have a database next to the main episerver db, for instance some API data, customer data...
        /// </summary>
        public string ExternalDB { get; set; }
    }
}
