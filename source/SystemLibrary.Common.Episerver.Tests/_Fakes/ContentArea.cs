using System.Collections.Generic;

using EPiServer.Core;

using FakeItEasy;

namespace SystemLibrary.Common.Episerver.Tests;

partial class Fakes
{
    public static ContentArea ContentArea()
    {
        var items = new List<ContentAreaItem> {
            ContentAreaItem()
        };

        var m = A.Fake<ContentArea>();

        A.CallTo(() => m.Items).Returns(items);
        A.CallTo(() => m.FilteredItems).Returns(items);

        return m;
    }
}