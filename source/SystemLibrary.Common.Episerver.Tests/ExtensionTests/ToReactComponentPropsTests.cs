
using System;
using System.Collections.Generic;

using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ToReactComponentPropsTests
{
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
        catch(Exception ex)
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

        Assert.IsTrue(html.Contains("data-rcssr=\"HelloWorld.123\""));

        Assert.IsTrue(html.Contains("{&quot;Title&quot;:null,&quot;Flag&quot;:false,&quot;Year&quot;:0}"), "Properties rendered are changed, some epi property is also printed?");
    }

    [TestMethod]
    public void Model_To_ReactProps_Success()
    {
        var model = new Model();
        
        var result = model.ReactServerSideRender(renderClientOnly: true);

        var html = result.ToString();

        // The component id is properly generated
        Assert.IsTrue(html.Contains("<div data-rcssr-id=\"k-3-33Model"));

        // The hidden input with props is generated with same id
        Assert.IsTrue(html.Contains("<input type='hidden' id=\"k-3-33Model"));

        // The component fullName is generated
        Assert.IsTrue(html.Contains(" data-rcssr=\"reactComponents.Model\""));

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
    public void BlockData_To_ExpandoObject_Success()
    {
        TempBlockData blockModel = null;
        IDictionary<string, object> props = blockModel.ToExpandoObject();
        Assert.IsTrue(props.Count == 0);

        blockModel = new TempBlockData();
        props = blockModel.ToExpandoObject();
        Assert.IsTrue(props.Count == 3);

        blockModel.Title = "Hello world";
        blockModel.Year = 20000;
        blockModel.Age = -1;

        // Ignored properties, built-in properties in epi is skipped by name
        blockModel.SortIndex = 100;
        blockModel.ShouldBeImplicitlyExported = "Ignored";

        props = blockModel.ToExpandoObject();

        // Skipped properties not returned hence 3
        Assert.IsTrue(props.Count == 3);
        Assert.IsTrue(props["Title"] == "Hello world");
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

    public class TempBlockData : BlockData
    {
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
