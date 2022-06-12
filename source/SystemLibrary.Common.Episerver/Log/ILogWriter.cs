namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// Implement ILogWriter in your application and register it as a Service in Startup.cs
    /// 
    /// Your logwriter will then be used whenever you use the static logger: Log.Errror(), Log.Warning(), Log.Write()
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Implement where Log.Error() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
        /// </summary>
        void Error(string message);
        /// <summary>
        /// Implement where Log.Warning() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
        /// </summary>
        void Warning(string message);
        /// <summary>
        /// Implement where Log.Write() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
        /// </summary>
        void Write(string message);
    }
}
