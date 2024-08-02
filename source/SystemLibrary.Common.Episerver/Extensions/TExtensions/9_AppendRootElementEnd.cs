using System.Text;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void AppendRootElementEnd(StringBuilder root, string tagName)
    {
        if (tagName != null)
            root.Append("</" + tagName + ">");
    }
}