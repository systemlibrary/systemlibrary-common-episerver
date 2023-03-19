using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class XhtmlStringExtensionsTests
{
    [TestMethod]
    public void Render_XhtmlString()
    {
        var xhtmlString = (XhtmlString)null;
        var expected = "";
        var result = xhtmlString.Render();
        Assert.IsTrue(result == expected, "result " + result + " is not equal " + expected);

        xhtmlString = new XhtmlString();
        expected = "";
        result = xhtmlString.Render();
        Assert.IsTrue(result == expected, "result " + result + " is not equal " + expected);

        xhtmlString = new XhtmlString("");
        expected = "";
        result = xhtmlString.Render();
        Assert.IsTrue(result == expected, "result " + result + " is not equal " + expected);

        xhtmlString = new XhtmlString("<div><a href=''>Hello world</a></div>");
        expected = "<div><a href=''>Hello world</a></div>";
        result = xhtmlString.Render();
        Assert.IsTrue(result == expected, "result " + result + " is not equal " + expected);
    }
}
