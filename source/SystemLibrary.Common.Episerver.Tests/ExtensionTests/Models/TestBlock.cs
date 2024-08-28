using System.Collections.Generic;

using EPiServer.Core;

using SystemLibrary.Common.Episerver.Users;

namespace SystemLibrary.Common.Episerver.Tests;

public class TestBlockInheritAndIdentity : TestBlock
{
    public CurrentUser TriggeringUser { get; set; }
    public int InheritedInt { get; set; }
}

public class TestBlock : BlockData
{
    public virtual IList<NestedTestBlock> InnerBlocks { get; set; }
    public virtual string Title { get; set; }

    public virtual bool Flag { get; set; }

    public int Age;

    public int Year { get; set; }
    string G { get; set; }
    internal static string H { get; set; }

    public int SortIndex { get; set; }
    public string ShouldBeImplicitlyExported { get; set; }
}
