using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Tests;

partial class Fakes
{
    public static ContentAreaItem ContentAreaItem()
    {
        return new ContentAreaItem()
        {
            ContentGroup = nameof(ContentAreaItem),
            ContentLink = new ContentReference(Random(), Random())
        };
    }
}