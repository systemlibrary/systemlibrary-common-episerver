namespace SystemLibrary.Common.Episerver
{
    public interface ILogWriter
    {
        void Error(string message);
        void Warning(string message);
        void Write(string message);
    }
}
