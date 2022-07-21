using System;

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

        Dump.Write("OK");
        try
        {
            Log.Write("OK");
        }
        catch(Exception ex)
        {
            Dump.Write(ex);
        }
      //  Log.Error("Hello world");

        //Assert.IsTrue(System.IO.File.Exists(FullPath));

//        Assert.IsTrue(System.IO.File.ReadAllText(FullPath).Contains("Hello world"));

        Dump.Clear();
    }
}
