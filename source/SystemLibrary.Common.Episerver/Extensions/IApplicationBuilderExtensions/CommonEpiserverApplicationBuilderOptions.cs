namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Contains application builder options that you can override if you want to
/// </summary>
public class CommonEpiserverApplicationBuilderOptions : Web.Extensions.CommonWebApplicationBuilderOptions
{
    public bool UseExceptionLogging = true;
    public bool UseEpiserverMapContentEndpoints = true;
}