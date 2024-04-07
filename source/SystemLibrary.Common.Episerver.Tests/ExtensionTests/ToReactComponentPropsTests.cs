
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
    public void Model_To_ReactProps_Success()
    {
        Model propsModel = null;

        var result = propsModel.ReactServerSideRender();

        Assert.IsTrue(result.Length == 0);

        propsModel = new Model();
        try
        {
            result = propsModel.ReactServerSideRender(renderClientOnly: true);

            Assert.IsTrue(false, "Should throw null exception on HttpContext");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.ToString().Contains("NullReferenceException"), ex.Message);
        }
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
