using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class IEnumerableContentReferenceExtensions
{
    [TestMethod]
    public void SelectFiltered_Success()
    {
        ContentArea contentArea = null;

        var filtered = contentArea.To<IContent>();
        
        Assert.IsNotNull(filtered);
        
        contentArea = new ContentArea();
        
        filtered = contentArea.To<IContent>();

        Assert.IsNotNull(filtered); 
    }
}
