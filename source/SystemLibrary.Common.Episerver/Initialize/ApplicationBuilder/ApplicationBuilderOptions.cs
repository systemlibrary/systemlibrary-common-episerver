namespace SystemLibrary.Common.Episerver.Initialize
{
    /// <summary>
    /// Contains application builder options that you can override if you want to
    /// </summary>
    public class ApplicationBuilderOptions : Web.Extensions.ApplicationBuilderOptions
    {
        public bool UseExceptionHandler = true;
        public bool UseRewriteEpiserverPathToEpiserverCms = true;
        public bool UseEpiserverEndspoints = true;
    }
}