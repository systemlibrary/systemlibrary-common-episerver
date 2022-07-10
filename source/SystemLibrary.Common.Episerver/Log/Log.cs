using EPiServer.Logging;

namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// One log class for your whole application
    /// 
    /// - Destination of where the log-message is saved is up to you to implement by implementing the interface ILogWriter in your application
    /// - After implementing ILogWriter, simply call Log.Error, Log.Warning or Log.Write to create a log message and it will passed to your implementation of ILogWriter
    /// 
    /// </summary>
    /// <example>
    /// Configure the log level by configuring episervers logging settings, usually in appSettings.json 
    /// <code class="language-csharp hljs">
    /// {  
    ///     "Logging" { 
    ///         "LogLevel" { 
    ///             "Default": "Error" 
    ///         }
    ///     } 
    /// }
    /// </code>
    /// </example>
    public static partial class Log
    {
        static ILogWriter _LogWriter;
        static ILogWriter LogWriter => _LogWriter != null ?
            _LogWriter : 
            (_LogWriter = Services.Get<ILogWriter>());

        public static void Error(object obj)
        {
            Write(obj, Level.Error);
        }

        public static void Warning(object obj)
        {
            Write(obj, Level.Warning);
        }

        public static void Write(object obj)
        {
            Write(obj, (Level)1000);
        }

        static void Write(object obj, Level level)
        {
            if (level != (Level)1000)
            {
                ILogger logger = LogManager.GetLogger();
                if (!logger.IsEnabled(level)) return;
            }

            var message = LogMessageBuilder.Get(obj, level);

            if (LogWriter == null)
            {
                Dump.Write("Common.Episerver.Log has been invoked due to an exception has been thrown or a call to Log.Error for example. Without you having registered an implementation of ILogWriter. Please implement 'SystemLibrary.Common.Episerver.ILogWriter' and register it in ConfigureServices(... services) { services.AddSingleton(typeof(ILogWriter), typeof('YourLogWriterClass')); Message was: " + message);
                return;
            }

            switch (level)
            {
                case Level.Warning:
                    LogWriter.Warning(message);
                    break;
                case Level.Error:
                    LogWriter.Error(message);
                    break;
                default:
                    LogWriter.Write(message);
                    break;
            }
        }
    }
}
