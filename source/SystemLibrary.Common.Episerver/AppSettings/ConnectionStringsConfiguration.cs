namespace SystemLibrary.Common.Episerver;

public class ConnectionStringsConfiguration
{
    public string EPiServerDB { get; set; }

    /// <summary>
    /// An additional optional db connection string in your appSettings.json file
    /// - If you have a database next to the main episerver db, for instance some API data, customer data...
    /// </summary>
    public string ExternalDB { get; set; } 
}
