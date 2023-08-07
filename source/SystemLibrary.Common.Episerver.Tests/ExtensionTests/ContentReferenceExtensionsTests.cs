using System;

using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ContentReferenceExtensionsTests
{
    [TestMethod]
    public void Is_And_IsNot_Success()
    {
        ContentReference contentReference = null;
        Assert.IsFalse(contentReference.Is(), "ContentReference Is() was true when null");
        Assert.IsTrue(contentReference.IsNot(), "ContentReference IsNot() was true when null");

        contentReference = new ContentReference();
        Assert.IsFalse(contentReference.Is(), "ContentReference Is() was true when empty");
        Assert.IsTrue(contentReference.IsNot(), "ContentReference IsNot() was true when empty");

        contentReference = new ContentReference(123);
        Assert.IsTrue(contentReference.Is(), "ContentReference has Id, but Is is false");
        Assert.IsFalse(contentReference.IsNot(), "ContentReference has Id, but IsNot is true");
    }

    [TestMethod]
    public void ToFriendlyUrl_Success()
    {
        ContentReference contentReference = null;
        var result = contentReference.ToFriendlyUrl();
        var expected = (string)null;
        Assert.IsTrue(result == expected, result + " is not equal " + expected);

        contentReference = new ContentReference();
        result = contentReference.ToFriendlyUrl();
        expected = null;
        Assert.IsTrue(result == expected, result + " is not equal " + expected);

        contentReference = new ContentReference(5);
        result = contentReference.ToFriendlyUrl();
        expected = "";
        Assert.IsTrue(result == expected, result + " is not equal " + expected);
    }
}
