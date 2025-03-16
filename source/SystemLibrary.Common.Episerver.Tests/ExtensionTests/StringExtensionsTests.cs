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
        Assert.IsTrue(result == "a", "Wrong single a " + result);

        url = "a";
        result = url.ToFriendlyUrl(false);
        Assert.IsTrue(result == "a", "Wrong A");

        url = "a";
        result = url.ToFriendlyUrl(true);
        Assert.IsTrue(result == "http://localhost/a", "Wrong protocol? " + result);

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

        url = ("/relative/path/" + "1234").ToFriendlyUrl(true);
        result = url.ToFriendlyUrl(convertToAbsolute: true);
        Assert.IsTrue(result == "http://localhost/relative/path/1234", result);

        url = "https://localhost.no:50000/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl(convertToAbsolute: true);
        Assert.IsTrue(result == "https://localhost.no:50000/relative-path/image.jpg/?quality=90", result);

        url = "https://localhost.no:50000/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl(convertToAbsolute: false);
        Assert.IsTrue(result == "/relative-path/image.jpg/?quality=90", result);

        url = "https://localhost.no:80/relative-path/image.jpg/?quality=90";
        result = url.ToFriendlyUrl(convertToAbsolute: true);
        Assert.IsTrue(result == "https://localhost.no:80/relative-path/image.jpg/?quality=90", result);

        url = "/relative-path/image.jpg/?quality=99";
        result = url.ToFriendlyUrl(convertToAbsolute: true);
        Assert.IsTrue(result == "http://localhost/relative-path/image.jpg/?quality=99", result);
    }

    [TestMethod]
    public void IsFile_Tests()
    {
        var data = (string)null;
        var expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "a";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong 1: " + data);

        data = "A";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong 2: " + data);

        data = "a/a/a/a/a";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong 3: " + data);

        data = "C:/Hello/World/";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.systemlibrary.com/hello/world/image?";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.png";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.png?quality=90";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.png?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/image.png?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/long/path/image.png";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/long/path/image.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/long/path/image.config";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "http://www.syslib.com/long/path/image.config?quality=190&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "image.config?quality=190&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.config?quality=190&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C://hello/world/image.config";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.config?quality=190&hello=world&.....";
        expected = true;

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "C:\\hello\\world\\image.config?q=.&a=b&c=.&362623632%2020%20quality=190&hello=world&.....";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "/image.config?q=.&a=b&c=.&362623632%2020%20quality=190&hello=world&.....";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "image.config?q=.&a=b&c=.&362623632%2020%20quality=190&hello=world&.....";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "image.config?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "../image.config?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "../image.config";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "../image.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "../../../image.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "../../../image.a";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "../../../image.";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no:8000/text////img.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no:8000/test////blog//anotherdepth/";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/test////blog//anotherdepth/";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/img";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/img.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
    }
}
