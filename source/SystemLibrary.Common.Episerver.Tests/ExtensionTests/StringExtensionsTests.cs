using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void Is_And_IsNot_Success()
    {
        var url = (string)null;
        var result = url.ToFriendlyUrl();
        Assert.IsTrue(result.IsNot());

        url = "";
        result = url.ToFriendlyUrl();
        Assert.IsTrue(result.IsNot());

        url = "a";
        result = url.ToFriendlyUrl();
        Assert.IsTrue(result == "a");

        url = "a";
        result = url.ToFriendlyUrl(false);
        Assert.IsTrue(result == "a");

        url = "a";
        result = url.ToFriendlyUrl(true);
        Assert.IsTrue(result == "http://localhost/a", result);

        url = "/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl(true);
        Assert.IsTrue(result == "http://localhost/relative-path/image.jpg/?quality=90", result);

        url = "/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl(false);
        Assert.IsTrue(result == "/relative-path/image.jpg/?quality=90", result);

        url = "/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl();
        Assert.IsTrue(result == "/relative-path/image.jpg/?quality=90", result);

        url = "https://localhost.no/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl();
        Assert.IsTrue(result == "https://localhost.no/relative-path/image.jpg/?quality=90", result);

        url = "https://localhost.no/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl(false);
        Assert.IsTrue(result == "/relative-path/image.jpg/?quality=90", result);

        url = "https://localhost.no/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl(convertToAbsolute: true);
        Assert.IsTrue(result == "https://localhost.no/relative-path/image.jpg/?quality=90", result);
    }
}
