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
    static void AppendRootElementEnd(StringBuilder root, string tagName)
    {
        if (tagName.Is())
            root.Append("</" + tagName + ">");
    }
}