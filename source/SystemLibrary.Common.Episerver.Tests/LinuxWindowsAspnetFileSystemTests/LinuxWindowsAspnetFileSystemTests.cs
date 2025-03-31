using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Framework;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class LinuxWindowsAspnetFileSystemTests
{
    [TestMethod]
    public void Null_Success()
    {
        var path = (string)null;

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsNull(res);
    }

    [TestMethod]
    public void Root_Success()
    {
        var path = "/";

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsTrue(res == "/");
    }

    [TestMethod]
    public void Squiggly_Root_Success()
    {
        var path = "~/";

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsTrue(res == "~/");
    }

    [TestMethod]
    public void Squiggly_Relative_Path_Is_Converted_To_Full_Path()
    {
        var path = "~/hello/world/";

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsTrue(res.StartsWith("C") && res.EndsWith("hello/world/"), "Error: " + res);
    }

    [TestMethod]
    public void Relative_Path_Is_Converted_To_Full_Path()
    {
        var path = "/hello/world/";

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsTrue(res.StartsWith("C") && res.EndsWith("hello/world/"), "Error: " + res);
    }

    [TestMethod]
    public void Relative_Path_Without_Starting_Slash_Is_Converted_To_Full_Path()
    {
        var path = "hello/world/";

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsTrue(res.StartsWith("C") && res.EndsWith("hello/world/"), "Error: " + res);
    }

    [TestMethod]
    public void Relative_Path_Without_Start_And_End_Slash_Is_Converted_To_Full_Path()
    {
        var path = "hello/world";

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsTrue(res.StartsWith("C") && res.EndsWith("hello/world"), "Error: " + res);
    }

    [TestMethod]
    public void Full_Path_From_Content_Root_Converted_To_Same_Url()
    {
        var path = Path.Combine(EnvironmentConfig.ContentRootPath, "hello/world");

        var sys = new LinuxAndWindowsAspNetFileSystem(null);

        var res = sys.MapPath(path);

        Assert.IsTrue(res == path, "Error: " + res);
    }
}
