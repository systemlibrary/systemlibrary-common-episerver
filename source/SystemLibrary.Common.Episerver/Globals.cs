namespace SystemLibrary.Common.Episerver
{
    internal static class Globals
    {
        internal const int ClientCacheDurationSeconds = 180;
        internal const int ServerCacheDurationSeconds = 180;

        internal const string AreaName = "SystemLibrary/Common/Episerver/";
        internal const string AreaPath = "/SystemLibrary/Common/Episerver/";

        internal static bool IsDeveloping => false;

        internal static string LibraryBasePath
        {
            get
            {
                return "C:\\syslib\\systemlibrary-common-episerver\\source\\SystemLibrary.Common.Episerver\\";
            }
        }

    }
}
