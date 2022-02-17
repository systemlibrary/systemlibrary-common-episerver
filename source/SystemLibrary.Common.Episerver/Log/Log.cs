using EPiServer.Logging;

namespace SystemLibrary.Common.Episerver
{
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
                throw new System.Exception("You are calling Common.Episerver.Log which is not registered. Register a 'LogWriter' that implements SystemLibrary.Common.Episerver.ILogWriter, then pass the Type of your new class and pass it to the method 'AddCommonEpiserver', inside your 'Startup.ConfigureServices'");

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
