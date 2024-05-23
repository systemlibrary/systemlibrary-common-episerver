using System;
using System.Collections;
using System.Collections.Generic;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ToReactPropsDictionaryTest
{
    [TestMethod]
    public void Convert_Null_To_Expando_Success()
    {
        TestBlock model = null;
        IDictionary<string, object> props = model.ToReactPropsDictionary();
        Assert.IsTrue(props.Count == 0);
    }

    [TestMethod]
    public void Convert_Simple_Poco_Returns_Properties_Success()
    {
        var model = new TestBlock();
        IDictionary<string, object> props = model.ToReactPropsDictionary();
        Assert.IsTrue(props.Count == 4, "Invalid count " + props.Count + ", did it include ignored properties?");
    }

    [TestMethod]
    public void Convert_Simple_Poco_With_Ignored_Properties_Should_Not_Return_Skipped_Properties_Success()
    {
        var model = new TestBlock();
        model.Title = "Hello world";
        model.Year = 20000;
        model.Age = -1;
        
        // Ignored properties, built-in properties in epi is skipped by name
        model.SortIndex = 100;
        model.ShouldBeImplicitlyExported = "Ignored";

        IDictionary<string, object> props = model.ToReactPropsDictionary();

        // Skipped properties not returned hence 4
        Assert.IsTrue(props.Count == 4, "Invalid count, included ignored properties? Youve added new ones? " + props.Count);
        Assert.IsTrue(props["Title"] == "Hello world");
    }

    [TestMethod]
    public void Convert_Simple_Poco_With_Dates_Success()
    {
        var model = new TestModel();
        model.B = "Hello world";
        model.C = 20000;
        model.E = DateTimeOffset.Now;

        IDictionary<string, object> props = model.ToReactPropsDictionary();

        Assert.IsTrue(props.Count == 4, "Count " + props.Count);
        Assert.IsTrue(props["B"] == "Hello world");
        Assert.IsTrue(props["C"] + "" == "20000");
        Assert.IsTrue(((DateTimeOffset)(props["E"])).Year == DateTimeOffset.Now.Year);
    }


    [TestMethod]
    public void Convert_BlockData_With_Nested_Blocks_ToExpando_Success()
    {
        TestBlock model = new TestBlock();
        model.Title = "hello";
        model.SortIndex = 100;
        model.Age = 99;
        model.InnerBlocks = new List<NestedTestBlock>
        {
            new NestedTestBlock()
            {
                Age = 8888,
                InnerTitle = "inner",
                InnerBool = true,
                InnerInt = -111,
                InnerXhtmlString = new XhtmlString("<p>Hello world</p>"),
                InnerRef = new ContentReference(55),
                InnerUrl = new Url("www.no"),
                InnerLinkItem = new LinkItem
                {
                     Href = "www.se",
                     Title = "SE 11",
                    Text = "LinkItemText"
                }
            },
             new NestedTestBlock()
            {
                InnerTitle = "innerinnerinner",
                InnerBool = true,
                Age = 7777,
                InnerInt = -111,
                InnerRef = new ContentReference(55),
                InnerUrl = new Url("www.no"),
                InnerLinkItem = new LinkItem
                {
                    Href = "www.se",
                    Title = "SE 22",
                    Text = "LinkItemText"
                }
            }
        };

        IDictionary<string, object> props = model.ToReactPropsDictionary();

        var json = props.Json();

        Assert.IsTrue(props.Count > 3);
        Assert.IsTrue(json.Contains("www.se"), "www.se missing");
        Assert.IsTrue(json.Contains("SE 22"), "SE 22 missing");
        Assert.IsTrue(json.Contains("-111"), "-111 missing");
        Assert.IsTrue(json.Contains("innerinnerinner"), "innerinnerinner missing");
        Assert.IsTrue(json.Contains("SE 11"), "SE 11 missing");
        Assert.IsTrue(json.Contains("www.no"), "www.no missing");
        Assert.IsTrue(json.Contains("LinkItemText"), "LinkItemText missing");
        Assert.IsTrue(!json.Contains("99"), "99 exists, it should be skipped, a field");
        Assert.IsTrue(!json.Contains("8888"), "8888 exists, it should be skipped, a field");
        Assert.IsTrue(!json.Contains("7777"), "7777 exists, it should be skipped, a field");
    }


    [TestMethod]
    public void Convert_List_To_Expando_Throw()
    {
        var model = new List<TestModel>();

        try
        {
            var exp1 = model.ToReactPropsDictionary();
            Assert.IsTrue(false, "Expando should throw on list");
        }
        catch
        {
            Assert.IsTrue(true, "List cannot be converted to expando object. Loop over the list yourself and convert each item");
        }
    }

    [TestMethod]
    public void Convert_Model_With_Dynamic_IEnumerable_To_ReactPropsDictionary()
    {
        var model = new TestModelDynamic()
        {
            IEnumerableDynamic = GetDynamicData(),
            IEnumerableStrings = GetDynamicStrings(),
            IEnumerableInts = GetDynamicInts()
        };

        var result = model.ToReactPropsDictionary();

        Assert.IsTrue(result["IEnumerableStrings"] != null);
        Assert.IsTrue(result["IEnumerableDynamic"] != null);
        Assert.IsTrue(result["IEnumerableInts"] != null);

        var ienumerableDynamicList = (IList)result["IEnumerableDynamic"];

        Assert.IsTrue(ienumerableDynamicList.Count == 3);

        var ienumerableIntsList = (IList)result["IEnumerableInts"];

        Assert.IsTrue(ienumerableIntsList.Count == 2);
    }

    static IEnumerable<int> GetDynamicInts()
    {
        yield return 50;
        yield return 500;
    }

    static IEnumerable<string> GetDynamicStrings()
    {
        yield return "Hello";
        yield return "World";
    }

    static IEnumerable<dynamic> GetDynamicData()
    {
        yield return new
        {
            Name = "Hello",
            LastName = "World",
            Age = 1001,
            Flag = true
        };
        yield return new
        {
            Name = "Hello2",
            LastName = "World2",
            Age = 1002,
            Flag = true
        };
        yield return new
        {
            Name = "Hello3",
            LastName = "World3",
            Age = 1003,
            Flag = true
        };
    }
}
