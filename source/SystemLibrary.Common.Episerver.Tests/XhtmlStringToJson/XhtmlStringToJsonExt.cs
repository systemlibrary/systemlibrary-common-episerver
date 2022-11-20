using System.Collections.Generic;

using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Cms.Attributes;

namespace SystemLibrary.Common.Episerver.Tests;

public class Car
{
    public string Name { get; set; }
    public XhtmlString Xhtml { get; set; }
}

[TestClass]
public class JsonEditAsObjectTests
{
    [TestMethod]
    public void Convert_String_To_Car_With_Xthmlstring_Property()
    {
        var html = "{ \"name\":\"Ferrari\", \"xhtml\":\"<h1>hello</h1>\"}";

        var car = html.JsonEditAsObject<Car>();

        Assert.IsTrue(car != null, "Null");
        Assert.IsTrue(car.Name == "Ferrari", "What" + car.Name);
        Assert.IsTrue(car.Xhtml != null && car.Xhtml.ToString().Contains("<h1>hello"));
    }

    [TestMethod]
    public void Convert_String_To_List_Of_Cars_With_Xthmlstring_Property()
    {
        var html = "[{ \"name\":\"Ferrari\", \"xhtml\":\"<h1>hello</h1>\"}, { \"name\":\"Opel\", \"xhtml\":\"<h2>world</h2>\"}]";

        var cars = html.JsonEditAsObject<List<Car>>();

        Assert.IsTrue(cars.Count == 2);
        foreach (var car in cars)
        {
            Assert.IsTrue(car.Name == "Ferrari" || car.Name == "Opel");
            Assert.IsTrue(car.Xhtml != null && car.Xhtml.ToString().Contains("<h"));
        }
    }
}
