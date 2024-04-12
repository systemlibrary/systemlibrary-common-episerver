using System.Collections;
using System.Dynamic;
using System.Text;

using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Transfer.Internal;
using EPiServer.Data;
using EPiServer.SpecializedProperties;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string GetSSRID(string id, object model, ExpandoObject props, string jsonProps)
    {
        // NOTE: We append "k-" as this Key is used for "data-rcssr-id"
        if (id.Is())
            return "k-" + id + "-" + jsonProps.Length;

        if (model is IContent icontent)
            return "k-" + icontent?.ContentLink?.ID + "-" + icontent.ContentLink?.WorkID + "-" + jsonProps.Length + "-" + props.Count();

        var contentData = model as ContentData;

        var ssrId = new StringBuilder("k-" + props.Count() + "-" + jsonProps.Length);

        if (jsonProps.Length > 8)
            ssrId.Append(jsonProps[3] + "" + jsonProps[4]);

        if (contentData != null)
        {
            ssrId.Append(contentData.ContentTypeID);
        }
        else
        {
            ssrId.Append(model.GetType()?.Name
                    .Replace("BlockViewModel", "BVM")
                    .Replace("ViewModel", "VM")
                    .Replace("Model", "MM")
                    .Replace("DynamicProxy", "DPX")
                    .Replace("AnonymousType", "ATY")
                    .Replace("<>", "")
                    .Replace("`", ""));
        }

        var propCount = props.Count();

        for (int i = 0; i < propCount; i++)
        {
            if (ssrId.Length > 128) break;

            var property = props.ElementAt(i);

            if (property.Value == null)
            {
                ssrId.Append("o");
                continue;
            }

            if (property.Value is StringBuilder sb)
            {
                if (sb?.Length > 0)
                {
                    if (sb?.Length > 5)
                    {
                        ssrId.Append("s" + sb.Length + "" + sb[3] + "" + sb[4] + "" + sb[sb.Length - 5]);
                    }
                    else if (sb.Length > 1)
                    {
                        ssrId.Append("s" + sb.Length + "" + sb[0] + "" + sb[1]);
                    }
                    else
                        ssrId.Append("s");
                }
            }

            else if (property.Value is string txt)
            {
                if (txt?.Length > 5)
                {
                    ssrId.Append("t" + txt.Length + "" + txt[3] + "" + txt[4] + "" + txt[txt.Length - 5]);
                }
                else if (txt.Length > 1)
                {
                    ssrId.Append("t" + txt?.Length + "" + txt[0] + "" + txt[1]);
                }
                else
                    ssrId.Append("t");
            }

            else if (property.Value is int number)
                ssrId.Append("i" + number);

            else if (property.Value is bool b)
                ssrId.Append("b" + (b ? "A" : "B"));

            else if (property.Value is ContentReference cr)
                ssrId.Append("cr" + cr?.ID + +cr?.WorkID);

            else if (property.Value is Url u)
                ssrId.Append("u" + u?.OriginalString?.Length);

            else if (property.Value is ContentArea ca)
                ssrId.Append("ca" + ca?.Count);

            else if (property.Value is LinkItemCollection lic)
                ssrId.Append("c" + lic?.Count);

            else if (property.Value is LinkItem li)
                ssrId.Append("li" + li.Href?.Length + li.Text?.Length);

            else if (property.Value is IEnumerable en)
                ssrId.Append("en" + property.Key[0]);

            else if (property.Value is DateTime dt)
                ssrId.Append("dt" + dt.Month + "-" + dt.Day + "-" + dt.Hour);

            else
            {
                ssrId.Append(property.Key[0]);
            }
        }

        return ssrId
            .Replace("<", "_")
            .Replace("&", "W")
            .Replace(">", "_")
            .Replace("`", "")
            .Replace("'", "")
            .Replace("\n", "")
            .Replace(" ", "z")
            .Replace("/", "--")
            .Replace("\\", "__")
            .Replace(":", "Q")
            .Replace(";", "Z")
            .Replace(";", "_")
            .Replace(Environment.NewLine, "")
            .Replace("\"", ".")
            .ToString();
    }
}