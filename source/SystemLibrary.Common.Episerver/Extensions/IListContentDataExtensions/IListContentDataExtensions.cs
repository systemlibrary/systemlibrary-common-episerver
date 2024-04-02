using System.Text;

using EPiServer.Core;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Builder;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

//public static class IListContentDataExtensions
//{
//    public static string Render<T>(this IList<T> iListOfContentData, bool forceCamelCase = false) where T : ContentData
//    {
//        if (iListOfContentData.IsNot()) return null;

//        var rendered = new StringBuilder();

//        foreach(ContentData contentData in iListOfContentData)
//        {
//            var props = contentData.ToExpandoObject(forceCamelCase);
//        }

//        return rendered.ToString();
//    }
//}
