namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Contains application builder options that you can override if you want to
/// </summary>
public class EpiserverAppBuilderOptions : Web.Extensions.WebApplicationBuilderOptions
{
    public bool UseExceptionLogging = true;
    public bool UseRewriteEpiserverPathToEpiserverCms = true;
    public bool UseEpiserverMapContentEndpoints = true;
}