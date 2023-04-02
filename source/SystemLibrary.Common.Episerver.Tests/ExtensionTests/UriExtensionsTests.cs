using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class UriExtensionsTests
{
    [TestMethod]
    public void Is_And_IsNot_Success()
    {
        var uri = (Uri)null;
        Assert.IsTrue(uri.IsNot());
        Assert.IsFalse(uri.Is());

        uri = new Uri("C:\\data\\logs\\");
        Assert.IsFalse(uri.IsNot());
        Assert.IsTrue(uri.Is());

        uri = new Uri("http://www.systemlibrary.com/path/?quality=80");
        Assert.IsFalse(uri.IsNot());
        Assert.IsTrue(uri.Is());
    }

    [TestMethod]
    public void ToFriendlyUrl_Success()
    {
        var uri = (Uri)null;
        var expected = (string)null;
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "C:\\Temp\\text.log";
        uri = new Uri(expected);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "http://www.systemlibrary.com/image.png";
        uri = new Uri(expected);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "https://www.systemlibrary.com/image.png";
        uri = new Uri(expected);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "http://www.systemlibrary.com/image.png/";
        uri = new Uri(expected);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "https://www.systemlibrary.com/image.png/";
        uri = new Uri(expected);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "http://www.systemlibrary.com/image.png/?quality=90";
        uri = new Uri(expected);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "https://www.systemlibrary.com/image.png/?quality=90";
        uri = new Uri(expected);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "www.systemlibrary.com/image.png/?quality=90";
        uri = new Uri(expected, UriKind.Relative);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "/image.png/?quality=90";
        uri = new Uri(expected, UriKind.Relative);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "image.png/?quality=90";
        uri = new Uri(expected, UriKind.Relative);
        Assert.IsTrue(uri.ToFriendlyUrl() == expected, uri + "");

        expected = "/image.png/?quality=90";
        uri = new Uri(expected, UriKind.Relative);
        Assert.IsTrue(uri.ToFriendlyUrl(true) == "http://localhost/image.png/?quality=90", uri + "");

        expected = "image.png/?quality=90";
        uri = new Uri(expected, UriKind.Relative);
        Assert.IsTrue(uri.ToFriendlyUrl(true) == "http://localhost/image.png/?quality=90", uri + "");
    }
}
