using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Tests.Models;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class JsonEditClassConverterTests
{
    [TestMethod]
    public void Convert_Class_To_JsonEdit_Json_Data()
    {
        var result = InvokeTestMethod(typeof(JsonEditCar));


        Assert.IsTrue(result != null && result.Length > 100);

        Assert.IsFalse(result.StartsWith("{"));
        Assert.IsFalse(result.StartsWith("["));
        Assert.IsTrue(result.Contains("Her kan du fylle inn fornavn"));
        Assert.IsTrue(result.Contains("Hello world"));
        Assert.IsTrue(result.Contains("Fra 0-100"));
        Assert.IsTrue(result.Contains("Disable"));
        Assert.IsTrue(result.Contains("Owner"));
        Assert.IsTrue(result.Contains("Car30"));

        Assert.IsFalse(result.Contains("A2"));

        var res = JsonDocument.Parse("{" + result + "}");

        Assert.IsTrue(res != null);
    }

    [TestMethod]
    public void Convert_Class_To_JsonEdit_Json_Data_Fails()
    {
        var result = InvokeTestMethod(typeof(JsonEditCarInvalid));

        Assert.IsTrue(result != null && result.Length > 100);
        var res = JsonDocument.Parse("{" + result + "}");

        Assert.IsFalse(result.Contains("PhoneNumbers"));
        Assert.IsFalse(result.Contains("PhoneNumberArray"));

        Assert.IsTrue(res != null);
    }

    static string InvokeTestMethod(Type type)
    {
        var testType = Type.GetType("SystemLibrary.Common.Episerver.Cms.Attributes.JsonEditPropertiesLoader, SystemLibrary.Common.Episerver");

        var method = testType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
           .Where(x => x.Name == "GetProperties")
           .FirstOrDefault();

        return method.Invoke(null, new object[] { type }) + "";
    } 
}
