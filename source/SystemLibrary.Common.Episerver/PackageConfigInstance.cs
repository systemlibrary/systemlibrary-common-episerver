using SystemLibrary.Common.Episerver;

/// <summary>
/// Exposed Package Configurations
/// <para>Exposes important parts of the appSettings you've set that are common for developers to "get the hand of" within C# code</para>
/// </summary>
public static class PackageConfigInstance
{
    public static string EPiServerDB => AppSettings.Current?.ConnectionStrings?.EPiServerDB;
    public static string ExternalDB => AppSettings.Current?.ConnectionStrings?.ExternalDB;
}