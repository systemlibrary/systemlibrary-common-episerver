using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ContentDataExtensionsTests
{
    [TestMethod]
    public void BlockData_Is_Displayable_Returns_False()
    {
        var contentData = new BlockData();

        var result = contentData.IsDisplayable();

        Assert.IsFalse(result, "Returned: " + result);
    }

    [TestMethod]
    public void BlockData_Is_Deleted_Due_To_Its_Name()
    {
        var contentData = new BlockData();

        var result = contentData.IsDeleted();

        Assert.IsTrue(result, "Returned: " + result);
    }
}
