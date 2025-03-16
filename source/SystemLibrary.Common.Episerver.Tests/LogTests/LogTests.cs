using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class LogTests 
{
    static string FullPath = @"C:\Logs\systemlibrary-common-framework-episerver-tests.log";

    [TestMethod]
    public void Log_Write_Dumps_Success()
    {
        Log.Clear();
        try
        {
            Log.Write("OK");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "Error occured: " + ex.Message);
            return;
        }
        Log.Error("Hello world");

        Thread.Sleep(33);

        Assert.IsTrue(System.IO.File.Exists(FullPath), "File not found");

        Assert.IsTrue(System.IO.File.ReadAllText(FullPath).Contains("Hello world"), "Missing text");

        Log.Clear();
    }
}
