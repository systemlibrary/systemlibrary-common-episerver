using System.Reflection;

namespace SystemLibrary.Common.Episerver.Tests;

public static class Initialize
{
    public static void Globals()
    {
        var assembly = typeof(WebApplicationInitializer).Assembly;

        var isUnitTesting = "IsUnitTesting";

        var globals = assembly.GetType(" SystemLibrary.Common.Episerver.Globals");

        var fields = globals.GetFields(BindingFlags.Static | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            if (field.Name == isUnitTesting)
            {
                field.SetValue(null, true);
            }
        }
    }
}
