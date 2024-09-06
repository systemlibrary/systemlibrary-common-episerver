namespace SystemLibrary.Common.Episerver;

internal static class Debug
{
    static bool? _Debugging;

    static bool Debugging
    {
        get
        {
            _Debugging ??= AppSettings.Current?.SystemLibraryCommonEpiserver?.Debug == true;

            return _Debugging.Value;
        }
    }

    // Requires debug=true and log level=debug or lower to be able to print debug information
    internal static void Log(object msg)
    {
        if (Debugging)
        {
            Dump.Write("Epi debug is 'true': " + msg);
        }
    }
}
