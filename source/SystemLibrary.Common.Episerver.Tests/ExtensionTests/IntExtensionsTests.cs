using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class IntExtensionsTests
{
    [TestMethod]
    public void To_Content()
    {
        var id = 0;

        var result = id.ToContent<BlockData>();

        Assert.IsTrue(result == null);
    }
}
