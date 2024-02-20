using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using EPiServer;
using EPiServer.Core;
using EPiServer.Shell.Web;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ObjectExtensions
{
    static string[] _BlackListedContentPropertiesLowered;
    static string[] BlackListedContentPropertiesLowered
    {
        get
        {
            if (_BlackListedContentPropertiesLowered == null)
            {
                var epiProperties = typeof(BlockData).GetProperties().Concat(typeof(PageData).GetProperties());

                _BlackListedContentPropertiesLowered = epiProperties.Select(p => p.Name)
                    .Concat(new string[]
                    {
                        "Properties",
                        "SortIndex",
                        "CurrentPage",
                        "CurrentBlock",
                        "EPiServer.Core.IContent.ContentTypeID",
                        "EPiServer.Core.IModifiedTrackable.IsModified",
                        "IModifiedTrackable.IsModified",
                        "ParentLinkReference",
                        "ShouldBeImplicitlyExported",
                        "MixinInstance",
                        "Property",
                        "ViewData"
                    })
                    .Select(p2 => p2.ToLower())
                    .ToArray();
            }
            return _BlackListedContentPropertiesLowered;
        }
    }

    public static ExpandoObject ToExpandoObject(this object model, bool forceCamelCase = false, params string[] ignorePropertyNames)
    {
        if (model == null) return new ExpandoObject();

        IDictionary<string, object> expando = new ExpandoObject();

        var type = model.GetType();

        if (!type.IsClass || type.IsInterface) throw new Exception("Cannot pass a non-class as model for react properties. Either create a class or an anonymous object and pass that with the variables you want as properties into your react component");

        var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField);

        if (properties == null || properties.Length == 0) return new ExpandoObject();

        var isContentDataModel = type.Name.EndsWithAny("Page", "Block", "BlockData", "PageData", "BlockProxy", "BlockDataProxy", "PageProxy", "PageDataProxy", "MediaProxy", "MediaData", "MediaDataProxy");

        foreach (var property in properties)
        {
            var name = property.Name;

            if (isContentDataModel && BlackListedContentPropertiesLowered.Contains(name.ToLower())) continue;

            if (ignorePropertyNames != null)
                if (ignorePropertyNames.Contains(name)) continue;

            if (name == "Property") continue;

            if (property.PropertyType.IsClass && (name.StartsWith("CurrentBlock") || name.StartsWith("CurrentPage") || name.StartsWith("CurrentMedia")))
                continue;

            if (name.StartsWith("EPiServer.")) continue;

            var value = property.GetValue(model);

            if (forceCamelCase)
            {
                if (name.Length <= 1)
                    name = name.ToLower();
                else
                    name = char.ToLowerInvariant(name[0]) + name.Substring(1);
            }

            if (value == null) 
                expando.Add(name, null);

            else if (value is MediaData mediaData)
                expando.Add(name, mediaData.ContentLink?.ToFriendlyUrl());

            else if (value is ContentArea contentArea)
            {
                expando.Add(name, contentArea.RenderStringBuilder());
            }

            else if (value is XhtmlString xHtmlString)
            {
                expando.Add(name, xHtmlString.RenderStringBuilder());
            }

            else if (value is Url url)
                expando.Add(name, url.ToFriendlyUrl());

            else if (value is Uri uri)
                expando.Add(name, uri.ToFriendlyUrl());

            else if (value is ContentReference contentReference)
                expando.Add(name, contentReference.ToFriendlyUrl());

            else if (value is PageData page)
                expando.Add(name, page.ToFriendlyUrl());

            else if (value is IEnumerable enumerable)
                expando.Add(name, enumerable);

            else if (value is Enum en)
                expando.Add(name, en.ToValue());

            else
                expando.Add(name, value);
        }

        return (ExpandoObject)expando;
    }
}