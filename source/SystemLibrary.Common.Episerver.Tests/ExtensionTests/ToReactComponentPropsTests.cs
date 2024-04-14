using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ToReactComponentPropsTests
{
    static ToReactComponentPropsTests()
    {
        Initialize.Globals();
    }

    [TestMethod]
    public void Render_Invalid_Configuration_Throws()
    {
        TestModel propsModel = new TestModel();

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
    public void Render_Invalid_Additional_Props_Throws()
    {
        TestModel propsModel = new TestModel();

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
    public void Render_AdditionalProps_Overwrites_Initial_Value_Success()
    {
        TestBlock model = new TestBlock();

        model.Title = "Hello";

        var result = model.ReactServerSideRender(new
        {
            Title = "Overwriting the hello from model.Title"
        }, renderClientOnly: true);

        var html = result.ToString();

        Assert.IsTrue(html.Contains("Overwriting the hello from model.Title"), "Wrong overwriting");
    }

    [TestMethod]
    public void Render_Simple_Model_With_ServerSide_Throws_On_ReactNET_Try_To_Render_Success()
    {
        var temp = new TestBlock();

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
    public void Render_Simple_Model_With_AdditionalEnumerableItems_Success()
    {
        var temp = new TestBlock();

        temp.InnerBlocks = new List<NestedTestBlock>() {
            new NestedTestBlock
            {
                 Age = 12345,
                 InnerTitle = "EnumerableTitle"
            }
        };

        var result = temp.ReactServerSideRender(new {
                enumerableItems = temp.InnerBlocks.Where(x => x !=null)
        },
        renderClientOnly: true);

        var html = result.ToString();

        // Check that IList from Model was printed correctly:
        Assert.IsTrue(html.Contains("{&quot;InnerBlocks&quot;:[{&quot;InnerTitle&quot;:&quot;EnumerableTitle&quot;,&quot;InnerInt&quot;:0,&quot;InnerBool&quot;:false,&quot;InnerXhtmlString&quot;:null,&quot;InnerRef&quot;:null,&quot;InnerUrl&quot;:null,&quot;InnerLinkItem&quot;:null}],&quot;Title&quot;:null,&quot;Flag&quot;:false,&quot;Year&quot;:0,&quot;"));

        // Check that the additional prop, as IEnumerable was printed correctly:
        Assert.IsTrue(html.Contains("enumerableItems&quot;:[{&quot;InnerTitle&quot;:&quot;EnumerableTitle&quot;,&quot;InnerInt&quot;:0,&quot;InnerBool&quot;:false,&quot;InnerXhtmlString&quot;:null,&quot;InnerRef&quot;:null,&quot;InnerUrl&quot;:null,&quot;InnerLinkItem&quot;:null}]}"));
    }

    [TestMethod]
    public void TempBlockData_To_ReactProps_Success()
    {
        var temp = new TestBlock();

        var result = temp.ReactServerSideRender(renderClientOnly: true, componentFullName: "A Brand New Component<>Name");

        var html = result.ToString();

        Assert.IsTrue(html.StartsWith("<div data-rcssr-id=\"k-4-55nn0oobBi0\"></div><input type='hidden' id=\"k-4-55nn0oobBi0\" "), "Beginning failed");

        Assert.IsTrue(html.Contains("data-rcssr=\"A Brand New Component<>Name\""), "Component full name has been tweaked internally");

        Assert.IsTrue(html.Contains("{&quot;InnerBlocks&quot;:null,&quot;Title&quot;:null,&quot;Flag&quot;:false,&quot;Year&quot;:0}"), "Properties rendered are changed, some epi property is also printed?");
    }

    [TestMethod]
    public void Model_To_ReactProps_Success()
    {
        var model = new TestModel();

        var result = model.ReactServerSideRender(renderClientOnly: true);

        var html = result.ToString();

        // The tagName and id is properly generated followed by input hidden
        Assert.IsTrue(html.StartsWith("<div data-rcssr-id=\"k-3-33.qTestMoi0E\"></div><input type='hidden' "), "Invalid beginning");

        // The hidden input with props is generated with same id
        Assert.IsTrue(html.Contains("<input type='hidden' id=\"k-3-33.qTestMoi0E\" "), "input");

        // The component fullName is generated, model, viewmodel is removed
        Assert.IsTrue(html.Contains(" data-rcssr=\"reactComponents.Test\""), "reactcomponents");

        // The object is converted to json and encoded
        Assert.IsTrue(html.Contains("&quot;B&quot;:null,&quot;C&quot;:0,&quot;E&quot;:&quot;0001-01-01&quot"));
    }

    [TestMethod]
    public void Model_With_Values_To_ReactProps_Success()
    {
        var model = new TestModel();

        model.A = "AAA";
        model.B = "BBB";
        model.C = 9999;

        var result = model.ReactServerSideRender(renderClientOnly: true);

        var html = result.ToString();

        Assert.IsTrue(!html.Contains("AAA"), "Field is part of the json");

        Assert.IsTrue(html.Contains("BBB"), "Property is missing from output");

        Assert.IsTrue(html.Contains("9999"), "Integer is missing from output");
    }
}
