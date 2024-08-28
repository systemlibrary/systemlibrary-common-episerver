using System.Text;

using React;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void RenderServerSideComponent(StringBuilder root, IReactComponent serverSideReactComponent, string ssrId, string tagName, bool renderClientSide)
    {
        if (serverSideReactComponent == null) return;

        root.Append(serverSideReactComponent.RenderHtml(false, true));
    }
}