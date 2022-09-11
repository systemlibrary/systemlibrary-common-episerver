using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ContentAreaExtensionsTests
{
    [TestMethod]
    public void Is_And_IsNot_Success()
    {
        ContentArea contentArea = null;
        Assert.IsFalse(contentArea.Is(), "Content area Is() was true when null");
        Assert.IsTrue(contentArea.IsNot(), "Content area IsNot() was true when null");

        contentArea = new ContentArea();
        Assert.IsFalse(contentArea.Is(), "Content area Is() was true when empty");
        Assert.IsTrue(contentArea.IsNot(), "Content area IsNot() was true when empty");

        contentArea = Fakes.ContentArea();
        Assert.IsTrue(contentArea.Is(), "ContentArea has 1 item, but Is is false");
        Assert.IsFalse(contentArea.IsNot(), "ContentArea has 1 item, but IsNot is true");
    }
}
