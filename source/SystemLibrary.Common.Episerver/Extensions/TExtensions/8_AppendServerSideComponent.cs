using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Microsoft.Identity.Client;

using React;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void RenderServerSideComponent(StringBuilder root, IReactComponent serverSideReactComponent)
    {
        if (serverSideReactComponent == null) return;

        root.Append(serverSideReactComponent.RenderHtml(false, true));

        // What? I know I need to add data-rcssr-id and its ssrId [key], but... 
        //if (tagName.IsNot() && renderClientSide)
        //{
        //    var space = content?.IndexOf(" ") ?? 0;
        //    if (space > 0 && space < 18)
        //    {
        //        content.Insert(space, " data-rcssr-id=\"" + key + "\"");

        //        if (debug)
        //            Log.Debug("Inserted data-rcssr-id=" + key + " at " + space);
        //    }
        //}
    }

}