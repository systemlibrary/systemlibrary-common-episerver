namespace SystemLibrary.Common.Episerver
{
    internal static class Globals
    {
        internal const int ClientCacheDurationSeconds = 180;
        internal const int ServerCacheDurationSeconds = 180;

        // Last part 'Cms' refers to the FolderName in root of this project
        internal const string AreaCms = "/SystemLibrary/CommonEpiserverCms";
        internal const string AreaFontAwesome = "/SystemLibrary/CommonEpiserverFontAwesome";

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
