using System.Collections;
using System.Text;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string GetSSRID(bool renderClientSide, string id, object model, Type modelType, IDictionary<string, object> props)
    {
        if (!renderClientSide) return null;

        // NOTE: We append a prefix and a suffix as this Key is used for "data-rcssr-id"
        if (id.Is()) return "i-" + id + "-" + props.Count;

        if (model is IContent icontent)
        {
            if (icontent.ContentLink == null || icontent.ContentLink.ID <= 0)
            {
                return "c-" + (icontent.Name.GetHashCode() % 100000) + icontent.ParentLink?.ID + "-" + icontent.ContentTypeID + "-" + props.Count;
            }
            return "c-" + icontent.ContentLink.ID + "-" + icontent.ContentLink.WorkID + "-" + props.Count;
        }

        var propCount = props.Count;

        var ssrId = new StringBuilder("k-" + propCount + "-" + props.Keys?.FirstOrDefault()?.Length);

        var contentData = model as ContentData;

        if (contentData != null)
        {
            ssrId.Append(contentData.ContentTypeID);
        }
        else
        {
            ssrId.Append(modelType.GetHashCode() % 1000);
        }

        for (int i = 0; i < propCount; i++)
        {
            var property = props.ElementAt(i);

            var value = property.Value;

            AppendValue(value, ssrId, property.Key[0]);
        }
        return ssrId.ToString();
    }

    static void AppendValue(object value, StringBuilder ssrId, char c)
    {
        if (ssrId.Length > 512) return;

        if (value == null)
        {
            ssrId.Append("o");
            return;
        }

        if (value is int number)
        {
            ssrId.Append(number);
        }

        else if (value is float f)
        {
            ssrId.Append(f);
        }

        else if (value is double d)
        {
            ssrId.Append(d);
        }

        else if (value is bool b)
        {
            ssrId.Append("B" + (b ? "A" : "B"));
        }

        else if (value is Url u)
        {
            ssrId.Append("u" + u?.OriginalString?.Length);
            if (u?.OriginalString?.Length > 2)
            {
                ssrId.Append(GetValidChar(u.OriginalString[u.OriginalString.Length - 2]));
            }
        }

        if (ssrId.Length > 192) return;

        if (value is StringBuilder sb)
        {
            if (sb.Length == 0) ssrId.Append("X1");

            ssrId.Append(sb.GetCompressedKey());
        }

        else if (value is string txt)
        {
            if (txt.Length == 0) ssrId.Append("X0");

            ssrId.Append(txt.GetCompressedKey());
        }

        else if (value is ContentReference cr)
            ssrId.Append(cr?.ID + "c" + cr?.WorkID);

        else if (value is ContentArea ca)
        {
            var count = ca?.Count;
            if (count > 0)
            {
                var sum = 0;
                var filteredItems = ca.FilteredItems;
                foreach (var item in filteredItems)
                    sum += ((ca.FilteredItems?.FirstOrDefault()?.ContentLink?.ID) ?? 0) * 11;

                ssrId.Append("CA" + sum);
            }
            else
            {
                ssrId.Append("CA0");
            }
        }

        else if (value is LinkItemCollection lic)
        {
            ssrId.Append("LC" + lic?.Count);

            if (lic?.Count > 0)
            {
                ssrId.Append("L" + (lic.FirstOrDefault()?.Href?.GetHashCode() % 10000));

                if (lic.Count > 1)
                    ssrId.Append("L" + lic.LastOrDefault()?.Href?.Length);
            }
        }

        else if (value is LinkItem li)
            ssrId.Append(li.Href?.Length + "LI" + (li.Text.GetHashCode() % 10000));

        else if (value is IEnumerable en)
            ssrId.Append("E" + c);

        else if (value is DateTime dt)
            ssrId.Append(dt.Day + "D" + dt.Month + "-" + dt.Hour);

        else if (value is DateTimeOffset dto)
            ssrId.Append(dto.Day + "DO" + dto.Month + "-" + dto.Hour);
        else
        {
            ssrId.Append(c);
        }
    }

    static string GetValidString(int l, char c1, char c2, char c3, char c4)
    {
        return GetValidChar(c3) + "" + GetValidChar(c1) + "-" + l + "" + GetValidChar(c2) + "" + GetValidChar(c4);
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