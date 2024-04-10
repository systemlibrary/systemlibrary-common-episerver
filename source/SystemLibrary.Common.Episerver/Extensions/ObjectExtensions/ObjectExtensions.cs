using EPiServer;
using EPiServer.Core;
using EPiServer.Enterprise.Transfer.Internal;
using EPiServer.Shell.Web;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc.Html;

using Microsoft.ClearScript.JavaScript;

using Org.BouncyCastle.Asn1.Cms;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ObjectExtensions
{
    static Type ContentDataType = typeof(ContentData);
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
                        "ParentLinkReferenceProperty",
                        "ParentLinkReference",
                        "ShouldBeImplicitlyExported",
                        "MixinInstance",
                        "Item",
                        "ContentTypeID",
                        "IsReadOnly",
                        "Property",
                        "ViewData",
                        "ReferencedPermanentLinkIds"
                    })
                    .Select(p2 => p2.ToLower())
                    .ToArray();
            }

            return _BlackListedContentPropertiesLowered;
        }
    }

    public static ExpandoObject ToExpandoObject(this object model, bool forceCamelCase = false, bool printNullValues = true, params string[] ignorePropertyNames)
    {
        if (model == null) return new ExpandoObject();

        IDictionary<string, object> expando = new ExpandoObject();

        var type = model.GetType();

        if (!type.IsClass || type.IsInterface) throw new Exception("Cannot pass a non-class as model for react properties. Either create a class or an anonymous object and pass that with the variables you want as properties in your react component");

        var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField);

        if (properties == null || properties.Length == 0) return new ExpandoObject();

        var isContentDataModel = type.Name.EndsWithAny("Proxy", "BlockData", "PageData", "MediaData", "ContentData");

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
            {
                if (printNullValues)
                    expando.Add(name, null);
            }

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

            else if (value is LinkItem linkItem)
            {
                var attributes = GetAttributesOfLinkItem(linkItem);
                expando.Add(name, attributes);
            }
            else if (value is IList<LinkItem> linkItems)
            {
                var linkItemAttributes = linkItems.Where(x => x.Attributes != null)?.Select(x => GetAttributesOfLinkItem(x));

                expando.Add(name, linkItemAttributes);
            }
            else if (value is ContentReference contentReference)
                expando.Add(name, contentReference.ToFriendlyUrl());

            else if (value is PageData page)
                expando.Add(name, page.ToFriendlyUrl());

            else if (value is IList iList)
            {
                var genericType = iList.GetType().GetFirstGenericType();
                if (genericType.Inherits(ContentDataType))
                {
                    var contentList = new List<IDictionary<string, object>>();

                    var listProperties = genericType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                    if (listProperties?.Length > 0)
                    {
                        foreach (var listItem in iList)
                        {
                            if (listItem == model) continue;

                            IDictionary<string, object> listItemExpando = new ExpandoObject();

                            foreach (var listProp in listProperties)
                            {
                                var listPropName = listProp.Name;

                                if (BlackListedContentPropertiesLowered.Contains(listPropName.ToLower())) continue;

                                if (ignorePropertyNames != null)
                                    if (ignorePropertyNames.Contains(listPropName)) continue;

                                if (listPropName == "Property") continue;

                                if (listProp.PropertyType.IsClass && (listPropName.StartsWith("CurrentBlock") || listPropName.StartsWith("CurrentPage") || listPropName.StartsWith("CurrentMedia")))
                                    continue;

                                if (listPropName.StartsWith("EPiServer.")) continue;

                                if (forceCamelCase)
                                {
                                    if (listPropName.Length <= 1)
                                        listPropName = listPropName.ToLower();
                                    else
                                        listPropName = char.ToLowerInvariant(listPropName[0]) + listPropName.Substring(1);
                                }
                                
                                var listPropValue = listProp.GetValue(listItem);

                                if (listPropValue == null)
                                {
                                    if (printNullValues)
                                        listItemExpando.Add(listPropName, listPropValue);

                                    continue;
                                }
                                else if (listPropValue is ContentArea listItemContentArea)
                                {
                                    listItemExpando.Add(listPropName, listItemContentArea.RenderStringBuilder());
                                }
                                else if (listPropValue is XhtmlString listItemXHtmlString)
                                {
                                    listItemExpando.Add(listPropName, listItemXHtmlString.RenderStringBuilder());
                                }
                                else if (listPropValue is Url listItemUrl)
                                    listItemExpando.Add(listPropName, listItemUrl.ToFriendlyUrl());

                                else if (listPropValue is Uri listItemUri)
                                    listItemExpando.Add(listPropName, listItemUri.ToFriendlyUrl());

                                else if (listPropValue is LinkItem listItemLinkItem)
                                {
                                    var attributes = GetAttributesOfLinkItem(listItemLinkItem);
                                    listItemExpando.Add(listPropName, attributes);
                                }
                                else if (listPropValue is IList<LinkItem> listItemLinkItems)
                                {
                                    var listItemLinkItemAttributes = listItemLinkItems.Where(x => x.Attributes != null)?.Select(x => GetAttributesOfLinkItem(x));

                                    listItemExpando.Add(listPropName, listItemLinkItemAttributes);
                                }
                                else if (listPropValue is ContentReference listItemContentReference)
                                    listItemExpando.Add(listPropName, listItemContentReference.ToFriendlyUrl());

                                else if (value is Enum en)
                                    listItemExpando.Add(listPropName, en.ToValue());

                                else if(listPropValue is int || listPropValue is bool || listPropValue is DateTime || listPropValue is string)
                                    listItemExpando.Add(listPropName, listPropValue);
                            }
                            
                            contentList.Add(listItemExpando);
                        }
                    }

                    expando.Add(name, contentList);
                }
                else
                {
                    expando.Add(name, iList);
                }
            }
            else if (value is IEnumerable enumerable)
                expando.Add(name, enumerable);
            else if (value is Enum en)
                expando.Add(name, en.ToValue());

            else if (value is MediaData mediaData)
                expando.Add(name, mediaData.ContentLink?.ToFriendlyUrl());

            else
                expando.Add(name, value);
        }

        return (ExpandoObject)expando;
    }

    static object GetAttributesOfLinkItem(LinkItem linkItem)
    {
        if (linkItem?.Attributes == null) return null;

        if (linkItem.Attributes.Count == 0) return null;

        var hasImage = linkItem.Attributes.ContainsKey("Image");
        var hasIcon = linkItem.Attributes.ContainsKey("Icon");
        var hasThumbnail = linkItem.Attributes.ContainsKey("Thumbnail");

        if (hasImage)
        {
            if (hasIcon)
            {
                if (hasThumbnail)
                {
                    return new
                    {
                        text = linkItem.Text,
                        title = linkItem.Title,
                        href = linkItem.Href.ToFriendlyUrl(),
                        target = linkItem.Target,
                        thumbnail = linkItem.Attributes["Thumbnail"],
                        icon = linkItem.Attributes["Icon"],
                        image = linkItem.Attributes["Image"],
                    };
                }
                return new
                {
                    text = linkItem.Text,
                    title = linkItem.Title,
                    href = linkItem.Href.ToFriendlyUrl(),
                    target = linkItem.Target,
                    icon = linkItem.Attributes["Icon"],
                    image = linkItem.Attributes["Image"],
                };
            }
            return new
            {
                text = linkItem.Text,
                title = linkItem.Title,
                href = linkItem.Href.ToFriendlyUrl(),
                target = linkItem.Target,
                image = linkItem.Attributes["Image"],
            };
        }

        if (hasThumbnail)
        {
            if (hasIcon)
            {
                return new
                {
                    text = linkItem.Text,
                    title = linkItem.Title,
                    href = linkItem.Href.ToFriendlyUrl(),
                    target = linkItem.Target,
                    thumbnail = linkItem.Attributes["Thumbnail"],
                    icon = linkItem.Attributes["Icon"]
                };
            }
            return new
            {
                text = linkItem.Text,
                title = linkItem.Title,
                href = linkItem.Href.ToFriendlyUrl(),
                target = linkItem.Target,
                thumbnail = linkItem.Attributes["Thumbnail"],
            };
        }

        if (hasIcon)
        {
            return new
            {
                text = linkItem.Text,
                title = linkItem.Title,
                href = linkItem.Href.ToFriendlyUrl(),
                target = linkItem.Target,
                icon = linkItem.Attributes["Icon"]
            };
        }

        return new
        {
            text = linkItem.Text,
            title = linkItem.Title,
            href = linkItem.Href.ToFriendlyUrl(),
            target = linkItem.Target,
        };
    }

    //static StringBuilder RenderIListContentItems(this List<ContentData> list)
    //{
    //    if (list == null) return null;

    //    var rendered = new StringBuilder();

    //    var iContentHtmlHelper = HtmlHelperFactory.Build<IContent>();

    //    foreach (var contentData in list)
    //    {
    //        if (contentData == null) continue;

    //        rendered.Append(iContentHtmlHelper.PropertyFor(x => contentData).ToString());
    //    }

    //    return rendered;
    //}
}