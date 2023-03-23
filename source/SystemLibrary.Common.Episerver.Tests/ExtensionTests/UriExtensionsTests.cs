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
}
