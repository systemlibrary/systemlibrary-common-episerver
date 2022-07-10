namespace SystemLibrary.Common.Episerver.Initialize;

/// <summary>
/// Contains application builder options that you can override if you want to
/// </summary>
public class IApplicationBuilderOptions : Web.Extensions.ApplicationBuilderOptions
{
    public bool UseExceptionLogging = true;
    public bool UseRewriteEpiserverPathToEpiserverCms = true;
    public bool UseEpiserverEndpoints = true;
}