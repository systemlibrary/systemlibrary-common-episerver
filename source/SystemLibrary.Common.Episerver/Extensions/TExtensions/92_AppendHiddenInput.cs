﻿using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Microsoft.Identity.Client;

using React;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void AppendHiddenInput(bool renderClientSide, string ssrId, string componentFullName, string jsonProps, HashSet<string> keys, StringBuilder root)
    {
        if (!renderClientSide) return;

        root.Append($"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />");

        //if (keys == null || !keys.Contains(ssrId))
        //{
        //    keys?.Add(ssrId);
        //}
    }
}