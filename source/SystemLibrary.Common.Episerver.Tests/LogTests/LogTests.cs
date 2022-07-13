using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class LogTests
{
    static string FullPath = @"C:\Logs\systemlibrary-common-episerver-tests.log";

    [TestMethod]
    public void Log_Write_Dumps_Success()
    {
        Dump.Clear();

        Log.Write("Hello world");

        Assert.IsTrue(System.IO.File.Exists(FullPath));

        Assert.IsTrue(System.IO.File.ReadAllText(FullPath).Contains("Hello world"));

        Dump.Clear();
    }
}
