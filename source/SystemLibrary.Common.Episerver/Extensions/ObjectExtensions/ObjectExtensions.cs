using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using EPiServer;
using EPiServer.Core;
using EPiServer.Enterprise.Transfer.Internal;
using EPiServer.Shell.Web;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc.Html;

using Microsoft.CodeAnalysis.Operations;

using SystemLibrary.Common.Episerver.Cms.Properties;
using SystemLibrary.Common.Net.Extensions;

using static Azure.Core.HttpHeader;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ObjectExtensions
{
    static Type SystemType = typeof(Type);
    static Type MessageType = typeof(Message);
    static Type ParentLinkReferenceType = typeof(ParentLinkReference);
    static Type CultureInfoType = typeof(CultureInfo);
    static Type PropertyUrlType = typeof(PropertyUrl);


    static string[] _BlackListedContentProperties;
    static string[] BlackListedContentProperties
    {
        get
        {
            if (_BlackListedContentProperties == null)
            {
                var epiProperties = typeof(BlockData).GetProperties().Concat(typeof(PageData).GetProperties());

                _BlackListedContentProperties = epiProperties.Select(p => p.Name)
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
                        "Guid",
                        "ContentReference",
                        "ContentTypeID",
                        "IsReadOnly",
                        "Property",
                        "ViewData",
                        "ReferencedPermanentLinkIds"
                    })
                    .ToArray();
            }

            return _BlackListedContentProperties;
        }
    }

    public static IDictionary<string, object> ToReactPropsDictionary(this object model, bool forceCamelCase = false, bool printNullValues = true, params string[] ignorePropertyNames)
    {
        if (model == null) return new Dictionary<string, object>();

        IDictionary<string, object> result = new Dictionary<string, object>();

        var type = model.GetType();

        if (!type.IsClass || type.IsInterface) throw new Exception("Cannot pass a non-class as model for react properties. Either create a class or an anonymous object and pass that with the variables you want as properties in your react component");

        var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);

        if (properties == null || properties.Length == 0) return new ExpandoObject();

        var isContentDataModel = type.Inherits(Globals.ContentDataType);

        foreach (var property in properties)
        {
            var name = property.Name;

            if (!IsPropertyElligibleAsProp(isContentDataModel, property, name, ignorePropertyNames))
            {
                continue;
            }

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
                    result.Add(name, null);
            }

            else if (value is ContentArea contentArea)
            {
                result.Add(name, contentArea.Render());
            }

            else if (value is XhtmlString xHtmlString)
            {
                result.Add(name, xHtmlString.Render());
            }

            else if (value is Url url)
                result.Add(name, url.ToFriendlyUrl());

            else if (value is Uri uri)
                result.Add(name, uri.ToFriendlyUrl());

            else if (value is LinkItem linkItem)
            {
                result.Add(name, GetAttributesOfLinkItem(linkItem));
            }
            else if (value is IList<LinkItem> linkItems)
            {
                var linkItemAttributes = linkItems.Where(x => x.Attributes != null)?.Select(x => GetAttributesOfLinkItem(x));

                result.Add(name, linkItemAttributes);
            }
            else if (value is ContentReference contentReference)
                result.Add(name, contentReference.ToFriendlyUrl());

            else if (value is PageData page)
                result.Add(name, page.ToFriendlyUrl());

            else if (value is IList iList)
            {
                var genericType = iList.GetType().GetFirstGenericType();
                if (genericType.Inherits(Globals.ContentDataType))
                {
                    var listItems = GetLoopableContentDataAsDictionary(model, forceCamelCase, printNullValues, ignorePropertyNames, value, iList, genericType);

                    result.Add(name, listItems);
                }
                else
                {
                    result.Add(name, iList);
                }
            }
            else if (value is IEnumerable enumerable)
            {
                var genericType = enumerable.GetType().GetFirstGenericType();
                if (genericType.Inherits(Globals.ContentDataType))
                {
                    var enumerableItems = GetLoopableContentDataAsDictionary(model, forceCamelCase, printNullValues, ignorePropertyNames, value, enumerable, genericType);

                    result.Add(name, enumerableItems);
                }
                else
                {
                    result.Add(name, enumerable);
                }
            }
            else if (value is Enum en)
                result.Add(name, en.ToValue());

            else if (value is MediaData mediaData)
                result.Add(name, mediaData.ContentLink?.ToFriendlyUrl());

            else
                result.Add(name, value);
        }

        return result;
    }

    static List<IDictionary<string, object>> GetLoopableContentDataAsDictionary(object model, bool forceCamelCase, bool printNullValues, string[] ignorePropertyNames, object value, IEnumerable list, Type genericType)
    {
        var hashCode = genericType.GetHashCode();
        var properties = Dictionaries.ReactPropPropertiesCache.TryGet(hashCode, () =>
        {
            return genericType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
        });

        var contentList = new List<IDictionary<string, object>>();

        if (properties == null || properties.Length == 0) return contentList;

        foreach (var content in list)
        {
            if (content == model) continue;

            IDictionary<string, object> item = new Dictionary<string, object>();

            foreach (var listProp in properties)
            {
                var listPropName = listProp.Name;

                if (!IsPropertyElligibleAsProp(true, listProp, listPropName, ignorePropertyNames))
                {
                    continue;
                }

                if (forceCamelCase)
                {
                    if (listPropName.Length <= 1)
                        listPropName = listPropName.ToLower();
                    else
                        listPropName = char.ToLowerInvariant(listPropName[0]) + listPropName.Substring(1);
                }

                var listPropValue = listProp.GetValue(content);

                if (listPropValue == null)
                {
                    if (printNullValues)
                        item.Add(listPropName, listPropValue);
                }
                else if (listPropValue is ContentArea listItemContentArea)
                {
                    item.Add(listPropName, listItemContentArea.Render());
                }
                else if (listPropValue is XhtmlString listItemXHtmlString)
                {
                    item.Add(listPropName, listItemXHtmlString.Render());
                }
                else if (listPropValue is Url listItemUrl)
                    item.Add(listPropName, listItemUrl.ToFriendlyUrl());

                else if (listPropValue is Uri listItemUri)
                    item.Add(listPropName, listItemUri.ToFriendlyUrl());

                else if (listPropValue is LinkItem listItemLinkItem)
                {
                    var attributes = GetAttributesOfLinkItem(listItemLinkItem);
                    item.Add(listPropName, attributes);
                }
                else if (listPropValue is IList<LinkItem> listItemLinkItems)
                {
                    var listItemLinkItemAttributes = listItemLinkItems.Where(x => x.Attributes != null)?.Select(x => GetAttributesOfLinkItem(x));

                    item.Add(listPropName, listItemLinkItemAttributes);
                }
                else if (listPropValue is ContentReference listItemContentReference)
                    item.Add(listPropName, listItemContentReference.ToFriendlyUrl());

                else if (value is Enum en)
                    item.Add(listPropName, en.ToValue());

                else if (listPropValue is int || listPropValue is bool || listPropValue is DateTime || listPropValue is string)
                    item.Add(listPropName, listPropValue);
            }

            contentList.Add(item);
        }

        return contentList;
    }

    static bool IsPropertyElligibleAsProp(bool isContentDataModel, PropertyInfo property, string name, string[] ignorePropertyNames)
    {
        if (!property.CanRead) return false;

        if (isContentDataModel && BlackListedContentProperties.Contains(name)) return false;

        if (ignorePropertyNames != null)
        {
            if (ignorePropertyNames.Contains(name))
            {
                return false;
            }
        }

        if (name == "Property") return false;

        var propertyType = property.PropertyType;

        if (propertyType.IsClass && (name.StartsWith("CurrentBlock") || name.StartsWith("CurrentPage") || name.StartsWith("CurrentMedia"))) return false;

        if (name.StartsWith("EPiServer.")) return false;

        if (propertyType == MessageType ||
            propertyType == ParentLinkReferenceType ||
            propertyType == SystemType ||
            propertyType == CultureInfoType ||
            propertyType == PropertyUrlType)
            return false;

        return true;
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