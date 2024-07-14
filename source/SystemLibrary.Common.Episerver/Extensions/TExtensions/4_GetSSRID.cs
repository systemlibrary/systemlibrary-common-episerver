﻿using System.Collections;
using System.Text;
using System.Xml.Linq;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

using Microsoft.Extensions.Primitives;

using static System.Net.Mime.MediaTypeNames;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string GetSSRID(bool renderClientSide, string id, object model, IDictionary<string, object> props, string jsonProps)
    {
        if (!renderClientSide) return null;

        // NOTE: We append a prefix and a suffix as this Key is used for "data-rcssr-id"
        if (id.Is())
            return "i-" + id + "-" + jsonProps.Length;

        if (model is IContent icontent)
            return "c-" + icontent?.ContentLink?.ID + "-" + icontent.ContentLink?.WorkID + "-" + jsonProps.Length;
        
        var ssrId = new StringBuilder("k-" + props.Count + "-" + jsonProps.Length);

        if (jsonProps.Length > 5)
            ssrId.Append(GetValidChar(jsonProps[3]) + "" + GetValidChar(jsonProps[4]));

        var contentData = model as ContentData;

        if (contentData != null)
        {
            ssrId.Append(contentData.ContentTypeID);
        }
        else
        {
            ssrId.Append(model.GetType().GetHashCode());
        }

        if (model is IContent content)
        {
            ssrId.Append("-" + content.Name?.GetHashCode());
        }

        var propCount = props.Count;

        for (int i = 0; i < propCount; i++)
        {
            var property = props.ElementAt(i);

            if (property.Value == null)
            {
                ssrId.Append("o");
                continue;
            }

            if (property.Value is int number)
            {
                ssrId.Append("i" + number);
                continue;
            }

            if (property.Value is bool b)
            {
                ssrId.Append("b" + (b ? "A" : "B"));
                continue;
            }

            if (property.Value is Url u)
            {
                ssrId.Append("u" + u?.OriginalString?.Length);
                continue;
            }

            if (ssrId.Length > 128)
            {
                continue;
            }

            if (ssrId.Length > 255) break;

            if (property.Value is StringBuilder sb)
            {
                if (sb == null)
                    continue;

                if (sb.Length == 0)
                    ssrId.Append("X1");

                else if (sb.Length > 255)
                {
                    ssrId.Append(GetValidString(sb.Length, sb[index: 3], sb[4], sb[5], sb[sb.Length - 5]));
                }
                else if (sb.Length > 5)
                    ssrId.Append(sb.GetHashCode() % 1000000);
                else
                    ssrId.Append(sb.Length + "" + sb.GetHashCode() % 10000);
            }

            else if (property.Value is string txt)
            {
                if (txt.Length == 0)
                    ssrId.Append("X0");

                else if (txt.Length > 255)
                {
                    ssrId.Append(GetValidString(txt.Length, txt[3], txt[4], txt[5], txt[txt.Length - 5]));
                }
                else if (txt.Length > 5)
                    ssrId.Append(txt.GetHashCode() % 1000000);
                else if (txt.Length >= 4)
                    ssrId.Append(GetValidString(txt.Length, txt[0], txt[1], txt[2], txt[3]));
                else if (txt.Length == 3)
                    ssrId.Append(GetValidString(txt.Length, txt[0], txt[1], txt[2], '-'));
                else
                    ssrId.Append(txt.Length + "" + (txt.GetHashCode() % 10000));
            }

            else if (property.Value is ContentReference cr)
                ssrId.Append(cr?.ID + "c" + cr?.WorkID);

            else if (property.Value is ContentArea ca)
                ssrId.Append("CA" + ca?.Count);

            else if (property.Value is LinkItemCollection lic)
                ssrId.Append("LC" + lic?.Count);

            else if (property.Value is LinkItem li)
                ssrId.Append(li.Href?.Length + "LI" + (li.Text.GetHashCode() % 100000));

            else if (property.Value is IEnumerable en)
                ssrId.Append("E" + property.Key[0]);

            else if (property.Value is DateTime dt)
                ssrId.Append("D" + dt.Day + "-" + dt.Hour);

            else
            {
                ssrId.Append(property.Key[0]);
            }
        }

        return ssrId.ToString();
    }

    static string GetValidString(int l, char c1, char c2, char c3, char c4)
    {
        return GetValidChar(c3) + "" + GetValidChar(c1) + "-" + l + "" + GetValidChar(c2)  + "" + GetValidChar(c4);
    }

    static char GetValidChar(char c)
    {
        if (c == '<') return '_';
        if (c == '&') return 'Q';
        if (c == '>') return '_';
        if (c == '`') return 'Q';
        if (c == '\n') return '-';
        if (c == ' ') return 'Z';
        if (c == '/') return '-';
        if (c == '\\') return '_';
        if (c == '"') return '.';
        if (c == ':') return 'q';
        if (c == ';') return 'z';
        if (c == '}') return '-';
        if (c == '{') return '_';
        if (c == '(') return '.';
        if (c == ')') return '.';

        if (c > 31 && c < 255) return c;

        return 'X';
    }
}