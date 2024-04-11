using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace SystemLibrary.Common.Episerver.Tests;


public class NestedTestBlock : BlockData
{
    public int Age;
    public virtual string InnerTitle { get; set; }
    public virtual int InnerInt { get; set; }
    public virtual bool InnerBool { get; set; }
    public virtual XhtmlString InnerXhtmlString { get; set; }
    public virtual ContentReference InnerRef { get; set; }
    public virtual Url InnerUrl { get; set; }
    public virtual LinkItem InnerLinkItem { get; set; }
}
