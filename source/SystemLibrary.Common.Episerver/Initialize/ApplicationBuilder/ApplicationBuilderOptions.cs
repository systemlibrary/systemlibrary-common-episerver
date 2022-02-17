namespace SystemLibrary.Common.Episerver.Initialize
{
    public class ApplicationBuilderOptions : Web.Extensions.ApplicationBuilderOptions
    {
        public bool UseExceptionHandler = true;
        public bool UseEpiserverRewriteToEpiserverCms = true;
        public bool UseEpiserverEndspoints = true;
    }
}