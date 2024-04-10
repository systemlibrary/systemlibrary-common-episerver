
using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ToReactComponentPropsTests
{
    static ToReactComponentPropsTests()
    {
        Initialize.Globals();
    }

    [TestMethod]
    public void Model_With_Client_And_Server_Rendering_Throws()
    {
        Model propsModel = new Model();

        try
        {
            var result = propsModel.ReactServerSideRender(renderClientOnly: true, renderServerOnly: true);

            Assert.IsTrue(false, "Should throw as params are invalid");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("cannot render"), "Wrong exception or message changed");
        }
    }

    [TestMethod]
    public void Model_With_NonClass_As_Additional_Props_Throws()
    {
        Model propsModel = new Model();

        var i = 100;

        try
        {
            var result = propsModel.ReactServerSideRender(i);

            Assert.IsTrue(false, "Should throw, additionalprops cannot be a single int");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("additionalProps"));
        }
    }

    [TestMethod]
    public void TempBlockData_To_ReactProps_ServerSideRendering_Success()
    {
        var temp = new TempBlockData();

        try
        {
            var result = temp.ReactServerSideRender(renderServerOnly: true);

            Assert.IsTrue(false, "ServerSide should invoke ReactJS.NET which is not initialized, which should throw");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("ReactJS.NET"));
        }
    }

    [TestMethod]
    public void TempBlockData_To_ReactProps_Success()
    {
        var temp = new TempBlockData();

        var result = temp.ReactServerSideRender(renderClientOnly: true, componentFullName: "HelloWorld.123");

        var html = result.ToString();

        Assert.IsTrue(html.StartsWith("<div data-rcssr-id=\"k-4-55nn0oobBi0\"></div><input"), "Beginning failed");

        Assert.IsTrue(html.Contains("data-rcssr=\"HelloWorld.123\""));

        Assert.IsTrue(html.Contains("{&quot;InnerBlocks&quot;:null,&quot;Title&quot;:null,&quot;Flag&quot;:false,&quot;Year&quot;:0}"), "Properties rendered are changed, some epi property is also printed?");
    }

    [TestMethod]
    public void Model_To_ReactProps_Success()
    {
        var model = new Model();

        var result = model.ReactServerSideRender(renderClientOnly: true);

        var html = result.ToString();

        // The component id is properly generated
        Assert.IsTrue(html.Contains("<div data-rcssr-id=\"k-3-33.QMMoi0E"), "id");
        // The hidden input with props is generated with same id
        Assert.IsTrue(html.Contains("<input type='hidden' id=\"k-3-33.QMMoi0E"), "input");

        // The component fullName is generated
        Assert.IsTrue(html.Contains(" data-rcssr=\"reactComponents.Model\""), "reactcomponents");

        // The object is converted to json and encoded
        Assert.IsTrue(html.Contains("&quot;B&quot;:null,&quot;C&quot;:0,&quot;E&quot;:&quot;0001-01-01&quot"));
    }

    [TestMethod]
    public void Model_With_Values_To_ReactProps_Success()
    {
        var model = new Model();

        model.A = "AAA";
        model.B = "BBB";
        model.C = 9999;

        var result = model.ReactServerSideRender(renderClientOnly: true);

        var html = result.ToString();

        Assert.IsTrue(!html.Contains("AAA"), "Field is part of the json");

        Assert.IsTrue(html.Contains("BBB"), "Property is missing from output");

        Assert.IsTrue(html.Contains("9999"), "Integer is missing from output");
    }

    [TestMethod]
    public void Model_To_ExpandoObject_Success()
    {
        Model propsModel = null;
        IDictionary<string, object> props = propsModel.ToExpandoObject();
        Assert.IsTrue(props.Count == 0);

        propsModel = new Model();
        props = propsModel.ToExpandoObject();
        Assert.IsTrue(props.Count == 3);

        propsModel.B = "Hello world";
        propsModel.C = 20000;
        propsModel.E = DateTimeOffset.Now;
        props = propsModel.ToExpandoObject();

        Assert.IsTrue(props.Count == 3);
        Assert.IsTrue(props["B"] == "Hello world");
        Assert.IsTrue(props["C"] + "" == "20000");
        Assert.IsTrue(((DateTimeOffset)(props["E"])).Year == DateTimeOffset.Now.Year);
    }

    [TestMethod]
    public void Model_With_IList_ContentData_To_Expando_Success()
    {
        TempBlockData data = new TempBlockData();
        data.Title = "hello";
        data.SortIndex = 100;
        data.Age = 99;
        data.InnerBlocks = new List<InnerBlockData>
        {
            new InnerBlockData()
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
             new InnerBlockData()
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
        
        IDictionary<string, object> props = data.ToExpandoObject();

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
    public void BlockData_To_ExpandoObject_Success()
    {
        TempBlockData blockModel = null;
        IDictionary<string, object> props = blockModel.ToExpandoObject();
        Assert.IsTrue(props.Count == 0);

        blockModel = new TempBlockData();
        props = blockModel.ToExpandoObject();
        Assert.IsTrue(props.Count == 4, "Invalid count " + props.Count);

        blockModel.Title = "Hello world";
        blockModel.Year = 20000;
        blockModel.Age = -1;

        // Ignored properties, built-in properties in epi is skipped by name
        blockModel.SortIndex = 100;
        blockModel.ShouldBeImplicitlyExported = "Ignored";

        props = blockModel.ToExpandoObject();

        // Skipped properties not returned hence 4
        Assert.IsTrue(props.Count == 4);
        Assert.IsTrue(props["Title"] == "Hello world");
    }

    [TestMethod]
    public void List_To_ExpandoObject_Throws()
    {
        var list = new List<Model>();

        try
        {
            var exp1 = list.ToExpandoObject();
            Assert.IsTrue(false, "Expando should throw on list");
        }
        catch
        {
            Assert.IsTrue(true, "List cannot be converted to expando object. Loop over the list yourself and convert each item");
        }
    }

    public class Model
    {
        public string A;
        public string B { get; set; }
        public int C { get; set; }
        public DateTime D;
        public DateTimeOffset E { get; set; }
        private DateTimeOffset F { get; set; }
        string G { get; set; }
        internal static string H { get; set; }
    }

    public class InnerBlockData : BlockData
    {
        public int Age;
        public virtual string InnerTitle { get; set; }
        public virtual int InnerInt { get; set; }
        public virtual bool InnerBool { get; set; }
        public virtual XhtmlString InnerXhtmlString { get; set; }
        public virtual ContentReference InnerRef { get; set; }
        public virtual Url InnerUrl { get; set; }
        public virtual LinkItem InnerLinkItem { get; set; }
    }

    public class TempBlockData : BlockData
    {
        public virtual IList<InnerBlockData> InnerBlocks { get; set; }
        public virtual string Title { get; set; }

        public virtual bool Flag { get; set; }

        public int Age;

        public int Year { get; set; }
        string G { get; set; }
        internal static string H { get; set; }

        public int SortIndex { get; set; }
        public string ShouldBeImplicitlyExported { get; set; }
    }
}
