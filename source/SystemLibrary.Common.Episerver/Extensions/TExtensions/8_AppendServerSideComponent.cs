using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Microsoft.Identity.Client;

using React;

using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void RenderServerSideComponent(StringBuilder root, IReactComponent serverSideReactComponent, string ssrId, string tagName, bool renderClientSide)
    {
        if (serverSideReactComponent == null) return;

        root.Append(serverSideReactComponent.RenderHtml(false, true));
    
        // tagName can be null if at least 'renderServerSide' is true, which is the case if it comes here
        // as the validation would throw otherwise
        // TagName is not, let's add the data-rcssr-id to the component that was outputted
        // Should be a space in first 18 characters
        if (tagName.IsNot() && renderClientSide)
        {
            var space = root?.IndexOf(" ") ?? 0;
            if (space == -1) return;

            if (space <= 1)
                space = root.IndexOf(" ", start: 2);

            if (space > 0 && space < 18)
                root.Insert(space, " data-rcssr-id=\"" + ssrId + "\"");
        }
    }
}