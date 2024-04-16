using System.Collections;
using System.Text;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string GetSSRID(bool renderClientSide, string id, object model, IDictionary<string, object> props, string jsonProps)
    {
        if (!renderClientSide) return null;

        // NOTE: We append "k-" as this Key is used for "data-rcssr-id"
        if (id.Is())
            return "k-" + id + "-" + jsonProps.Length;

        if (model is IContent icontent)
            return "k-" + icontent?.ContentLink?.ID + "-" + icontent.ContentLink?.WorkID + "-" + jsonProps.Length;

        var contentData = model as ContentData;

        var ssrId = new StringBuilder("k-" + props.Count + "-" + jsonProps.Length);

        if (jsonProps.Length > 8)
            ssrId.Append(GetValidChar(jsonProps[3]) + "" + GetValidChar(jsonProps[4]));

        if (contentData != null)
        {
            ssrId.Append(contentData.ContentTypeID);
        }
        else
        {
            ssrId.Append(model.GetType()?.Name
                    .Replace("BlockViewModel", "BVM")
                    .Replace("ViewModel", "VM")
                    .Replace("Component", "C")
                    .Replace("Model", "M")
                    .Replace("DynamicProxy", "DP")
                    .Replace("AnonymousType", "AT")
                    .Replace("<>", "")
                    .Replace("`", ""));
        }

        var propCount = props.Count;

        for (int i = 0; i < propCount; i++)
        {
            if (ssrId.Length > 92) break;

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
                    ssrId.Append(sb.Length);

                    if (sb?.Length > 5)
                    {
                        ssrId.Append(GetValidString(sb.Length, sb[3], sb[4], sb[sb.Length - 5]));
                    }
                    else if (sb.Length > 1)
                    {
                        ssrId.Append(GetValidString(sb.Length, sb[0], sb[1], sb[sb.Length - 2]));
                    }
                    else
                        ssrId.Append("s");
                }
            }

            else if (property.Value is string txt)
            {
                ssrId.Append(txt.Length);
                if (txt?.Length > 5)
                {
                    ssrId.Append(GetValidString(txt.Length, txt[3], txt[4], txt[txt.Length - 5]));
                }
                else if (txt.Length > 1)
                {
                    ssrId.Append(GetValidString(txt.Length, txt[0], txt[1], txt[txt.Length - 2]));
                }
                else
                    ssrId.Append("t");
            }

            else if (property.Value is int number)
                ssrId.Append("i" + number);

            else if (property.Value is bool b)
                ssrId.Append("b" + (b ? "A" : "B"));

            else if (property.Value is ContentReference cr)
                ssrId.Append("c" + cr?.ID + +cr?.WorkID);

            else if (property.Value is Url u)
                ssrId.Append("u" + u?.OriginalString?.Length);

            else if (property.Value is ContentArea ca)
                ssrId.Append("CA" + ca?.Count);

            else if (property.Value is LinkItemCollection lic)
                ssrId.Append("LC" + lic?.Count);

            else if (property.Value is LinkItem li)
                ssrId.Append("LI" + li.Href?.Length + li.Text?.Length);

            else if (property.Value is IEnumerable en)
                ssrId.Append("en" + property.Key[0]);

            else if (property.Value is DateTime dt)
                ssrId.Append("DT" + dt.Day + "-" + dt.Hour);

            else
            {
                ssrId.Append(property.Key[0]);
            }
        }

        return ssrId.ToString();
    }

    static string GetValidString(int l, char c1, char c2, char c3)
    {
        return l + "-" + GetValidChar(c1) + "" + GetValidChar(c2) + "" + GetValidChar(c3);
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

        return c;
    }
}