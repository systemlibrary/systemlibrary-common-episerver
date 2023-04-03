using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using EPiServer;
using EPiServer.Core;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ObjectExtensions
{
    static string[] BlackListedContentProperties = new string[]
    {
            "ParentLinkReference",
            "ContentTypeID",
            "IsModified",
            "IModifiedTrackable.IsModified",
            "CurrentPage",
            "CurrentBlock",
            "Category",
            "Created",
            "CreatedBy",
            "Changed",
            "SetChangedOnPublish",
            "ChangedBy",
            "Saved",
            "DeletedBy",
            "Deleted",
            "Name",
            "ContentLink",
            "ParentLink",
            "ContentGuid",
            "EPiServer.Core.IContent.ContentTypeID",
            "IsDeleted",
            "Language",
            "ExistingLanguages",
            "MasterLanguage",
            "ContentAssetsID",
            "Status",
            "IsPendingPublish",
            "StartPublish",
            "StopPublish",
            "ShouldBeImplicitlyExported",
            "MixinInstance",
            "Property",
            "IsReadOnly",
            "Item",
            "ViewData"
    };

    public static ExpandoObject ToExpandoObject(this object model, bool forceCamelCase = false, params string[] ignorePropertyNames)
    {
        if (model == null) return new ExpandoObject();

        IDictionary<string, object> expando = new ExpandoObject();

        var type = model.GetType();

        if (!type.IsClass || type.IsInterface) throw new Exception("Cannot pass a non-class as model for react properties. Either create a class or an anonymous object and pass that with the variables you want as properties into your react component");

        var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField);

        if (properties == null || properties.Length == 0) return new ExpandoObject();

        var isContentDataModel = type.Name.EndsWithAny("BlockData", "PageData", "BlockProxy", "BlockDataProxy", "PageProxy", "PageDataProxy", "MediaData", "MediaDataProxy");

        var blackListedLowered = BlackListedContentProperties.Select(x => x.ToLower());

        foreach (var property in properties)
        {
            var name = property.Name;

            if (isContentDataModel && blackListedLowered.Contains(name.ToLower())) continue;

            if (ignorePropertyNames != null)
                if (ignorePropertyNames.Contains(name)) continue;

            if (name == "Property") continue;

            if (property.PropertyType.IsClass && (name.StartsWith("CurrentBlock") || name.StartsWith("CurrentPage")))
                continue;

            var value = property.GetValue(model);

            if (forceCamelCase)
            {
                if (name.Length <= 1)
                    name = name.ToLower();
                else
                    name = char.ToLowerInvariant(name[0]) + name.Substring(1);
            }

            if (value is XhtmlString xHtmlString)
                expando.Add(name, xHtmlString.Render());

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

            else if (value is ContentArea contentArea)
                expando.Add(name, contentArea.Render());    // TODO: Consider read each fragment, render each fragment...

            else if (value is Enum en)
                expando.Add(name, en.ToValue());

            else
                expando.Add(name, value);
        }

        return (ExpandoObject)expando;
    }
}
