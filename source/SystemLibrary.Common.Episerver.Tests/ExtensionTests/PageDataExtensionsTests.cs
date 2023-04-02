using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class PageDataExtensionsTests
{
    [TestMethod]
    public void Is_And_IsNot_Success()
    {
        var data = (PageData)null;
        var expected = true;
        Assert.IsTrue(data.IsNot() == expected);

        data = new PageData();
        expected = false;
        Assert.IsTrue(data.IsNot() == expected);

        data = (PageData)null;
        expected = false;
        Assert.IsTrue(data.Is() == expected);

        data = new PageData();
        expected = true;
        Assert.IsTrue(data.Is() == expected);

        data = (PageData)null;
        Assert.IsTrue(data.ToFriendlyUrl() == null);

        data = new PageData(new PageReference(5));
        Assert.IsTrue(data.ToFriendlyUrl() == "");
    }
}
