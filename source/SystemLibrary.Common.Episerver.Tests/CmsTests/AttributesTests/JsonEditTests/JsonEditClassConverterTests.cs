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
        var result = InvokeGetPropertySchemaMethod(typeof(JsonEditCar));

        Assert.IsTrue(result != null && result.Length > 100);

        Assert.IsFalse(result.StartsWith("{"));
        Assert.IsFalse(result.StartsWith("["));
        Assert.IsTrue(result.Contains("Fill in firstname"));
        Assert.IsTrue(result.Contains("Hello world"));
        Assert.IsTrue(result.Contains("From 0-100"));
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
        var result = InvokeGetPropertySchemaMethod(typeof(JsonEditCarInvalid));

        Assert.IsTrue(result != null, "Result is null");
        Assert.IsTrue(result.Length > 100, "Result is too short: " + result.Length);

        var res = JsonDocument.Parse("{" + result + "}");

        Assert.IsTrue(res != null, "Error parsing result to json document");

        Assert.IsFalse(result.Contains("PhoneNumbers"), "Does contain PhoneNumbers, which it shouldnt");
        Assert.IsFalse(result.Contains("PhoneNumberArray"), "Does contain PhoneNumberArray, which it shouldnt");

    }

    static string InvokeGetPropertySchemaMethod(Type type)
    {
        var testType = Type.GetType(typeName: "SystemLibrary.Common.Episerver.Attributes.JsonEditPropertiesLoader, SystemLibrary.Common.Episerver");

        if (testType == null)
            throw new Exception("SystemLibrary.Common.Episerver.Attributes.JsonEditPropertiesLoader is not loaded or type is renamed");

        var method = testType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
           .Where(x => x.Name == "GetPropertySchema")
           .FirstOrDefault();

        if (method == null)
            throw new Exception("Method 'GetProperties' is renamed or do not exist");

        return method.Invoke(null, new object[] { type }) + "";
    }
}
