using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class LogTests
{
    static string FullPath = @"C:\Logs\systemlibrary-common-episerver-unit-tests.txt";

    [TestMethod]
    public void Log_Write_Dumps_Success()
    {
        Dump.Clear();
        try
        {
            Log.Write("OK");
        }
        catch(Exception ex)
        {
            Assert.IsTrue(false, "Error occured: " + ex.Message);
            return;
        }
        Log.Error("Hello world");

        Assert.IsTrue(System.IO.File.Exists(FullPath));

        Assert.IsTrue(System.IO.File.ReadAllText(FullPath).Contains("Hello world"));

        Dump.Clear();
    }
}
