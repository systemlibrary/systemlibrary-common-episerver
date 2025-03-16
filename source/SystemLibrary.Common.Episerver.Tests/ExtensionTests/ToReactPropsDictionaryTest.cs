using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ToReactPropsDictionaryTest
{
    static MethodInfo ToPropsDictionary;
    static ToReactPropsDictionaryTest()
    {
        var type = typeof(Extensions.ObjectExtensions);

        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                   .Where(x => x.Name == "ToPropsDictionary")
                   .FirstOrDefault();

        if (method == null)
            throw new Exception("Method 'ToPropsDictionary' is not existing on type: " + type.Name);

        ToPropsDictionary = method;
    }

    [TestMethod]
    public void Convert_Null_To_Expando_Success()
    {
        TestBlock model = null;
        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, true, null }) as IDictionary<string, object>;
        Assert.IsTrue(props.Count == 0);
    }

    [TestMethod]
    public void Convert_Inherited_And_CurrentUser_Props_Success()
    {
        TestBlockInheritAndIdentity model = new TestBlockInheritAndIdentity();

        model.TriggeringUser = new Users.AppCurrentUser();
        model.InheritedInt = 999;
        model.Year = 888;

        var props = ToPropsDictionary.Invoke(null, new object[] { model, true, true, null }) as IDictionary<string, object>;

        Assert.IsTrue(props.Count == 6, "Inherited props missing? " + props.Count);

        Assert.IsTrue(props["year"] != null);
        Assert.IsTrue(props["inheritedInt"] != null);

        Assert.IsTrue(props.ContainsKey("triggeringUser"), "TriggeringUser missing");

        var triggeringUser = props["triggeringUser"] as IDictionary<string, object>;

        Assert.IsTrue(triggeringUser != null, "TriggeringUser is null");

        Assert.IsTrue(triggeringUser.ContainsKey("isCmsUser"), "1");
        Assert.IsTrue(triggeringUser.ContainsKey("isCmsUser"), "2");
        Assert.IsTrue(triggeringUser.ContainsKey("name"), "3");
        Assert.IsTrue(triggeringUser.ContainsKey("givenName"), "4 ");
        Assert.IsTrue(triggeringUser.ContainsKey("surname"), "5 ");
        Assert.IsTrue(triggeringUser.ContainsKey("phoneNumber"), "6 ");
        Assert.IsTrue(triggeringUser.ContainsKey("email"), "7");
        Assert.IsTrue(triggeringUser.ContainsKey("comment"), "8");
    }

    [TestMethod]
    public void Convert_Simple_Poco_Returns_Properties_Success()
    {
        var model = new TestBlock();
        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, true, null }) as IDictionary<string, object>;
        Assert.IsTrue(props.Count == 4, "Invalid count " + props.Count + ", did it include ignored properties?");
    }

    [TestMethod]
    public void Convert_Simple_Poco_Force_Camel_Case_Properties_Returns_Properties_Success()
    {
        var model = new TestBlock();

        model.Title = "Hello";

        var forceCamelCase = true;

        var props = ToPropsDictionary.Invoke(null, new object[] { model, forceCamelCase, true, null }) as IDictionary<string, object>;
        try
        {
            Assert.IsTrue(props["Title"] == null);
            Assert.IsTrue(false, "Should throw, Title should not be in props as camelCase is forced");
        }
        catch
        {
            Assert.IsTrue(true, "Throws then 'Title' do not exist in props");
        }

        Assert.IsTrue(props["title"] != null);
    }

    [TestMethod]
    public void Convert_Simple_Poco_Skipping_Two_Properties_Returns_Properties_Success()
    {
        var model = new TestBlock();

        var skip = new string[] { "Flag", "Title" };

        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, true, skip }) as IDictionary<string, object>;

        Assert.IsTrue(props.Count == 2, "Invalid count " + props.Count + ", did it include ignored properties?");
    }


    [TestMethod]
    public void Convert_Simple_Poco_Skip_Print_Null_Properties_Returns_Properties_Success()
    {
        var model = new TestBlock();

        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, false, null }) as IDictionary<string, object>;

        Assert.IsTrue(props.Count == 2, "Invalid count " + props.Count + ", did it include null properties?");

        model.Title = "Hello";

        props = ToPropsDictionary.Invoke(null, new object[] { model, false, false, null }) as IDictionary<string, object>;

        Assert.IsTrue(props.Count == 3, "Invalid count " + props.Count + ", did it include null properties?");
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

        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, true, null }) as IDictionary<string, object>;

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

        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, true, null }) as IDictionary<string, object>;

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

        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, true, null }) as IDictionary<string, object>;

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
            var props = ToPropsDictionary.Invoke(null, [model]) as IDictionary<string, object>;
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

        var props = ToPropsDictionary.Invoke(null, new object[] { model, false, true, null }) as IDictionary<string, object>;

        Assert.IsTrue(props["IEnumerableStrings"] != null);
        Assert.IsTrue(props["IEnumerableDynamic"] != null);
        Assert.IsTrue(props["IEnumerableInts"] != null);

        var ienumerableDynamicList = (IList)props["IEnumerableDynamic"];

        Assert.IsTrue(ienumerableDynamicList.Count == 3);

        foreach (var dynamicDataItem in ienumerableDynamicList)
        {
            dynamic casted = dynamicDataItem;
            string[] strings = (string[])casted.Strings;

            Assert.IsTrue(strings.Length == 2);

            int age = (int)casted.Age;
            Assert.IsTrue(age > 0);
        }

        var ienumerableIntsList = (IList)props["IEnumerableInts"];

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

    static IEnumerable<int> RandomInts()
    {
        yield return 10;
        yield return 15;
        yield return 20;
    }

    static IEnumerable<dynamic> GetDynamicData()
    {
        yield return new
        {
            Name = "Hello",
            LastName = "World",
            Age = 1001,
            Year = DateTime.Now,
            Flag = true,
            Strings = new string[] { "Hello", "World" },
            IEnum = RandomInts()
        };
        yield return new
        {
            Name = "Hello2",
            LastName = "World2",
            Age = 1002,
            Year = DateTime.Now,
            Flag = true,
            Strings = new string[] { "Hello", "World" },
            IEnum = RandomInts()
        };
        yield return new
        {
            Name = "Hello3",
            LastName = "World3",
            Age = 1003,
            Year = DateTime.Now,
            Flag = true,
            Strings = new string[] { "Hello", "World" },
            IEnum = RandomInts()
        };
    }
}
