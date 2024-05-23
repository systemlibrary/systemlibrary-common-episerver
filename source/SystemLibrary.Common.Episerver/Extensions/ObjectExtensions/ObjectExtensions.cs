using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;

using EPiServer;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;
using EPiServer.Enterprise.Transfer.Internal;
using EPiServer.Security.Internal;
using EPiServer.Shell.Web;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc.Html;

using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Operations;

using SystemLibrary.Common.Episerver.Properties;
using SystemLibrary.Common.Net.Extensions;

using static Azure.Core.HttpHeader;

namespace SystemLibrary.Common.Episerver.Extensions;

public static class ObjectExtensions
{
    static Type SystemType;
    static Type MessageType;
    static Type ParentLinkReferenceType;
    static Type CultureInfoType;
    static Type PropertyUrlType;
    static ConcurrentDictionary<int, PropertyInfo[]> ReactPropPropertiesCache;
    static ConcurrentDictionary<int, PropertyInfo[]> IdentityUserProperties;

    static ObjectExtensions()
    {
        SystemType = typeof(Type);
        MessageType = typeof(Message);
        ParentLinkReferenceType = typeof(ParentLinkReference);
        CultureInfoType = typeof(CultureInfo);
        PropertyUrlType = typeof(PropertyUrl);

        ReactPropPropertiesCache = new ConcurrentDictionary<int, PropertyInfo[]>();
        IdentityUserProperties = new ConcurrentDictionary<int, PropertyInfo[]>();
    }

    static string[] WhiteListedCustomProperties = new string[]
    {
        "Id",
        "Current",
        "Comment",
        "Email",
        "PhoneNumber"
    };

    static string[] BlackListedCustomProperties = new string[]
    {
        "CurrentPage",
        "CurrentBlock",
        "Password",
        "PasswordHash",
        "PasswordQuestion",
        "Properties",
        "SecurityStamp",
        "LastLoginDate",
        "IdToken",
        "IdTokenHint",
        "PhoneNumberConfirmed",
        "AccessFailedCount",
        "ProviderName",
        "IsAuthenticated",
        "IsAdministrator"
    };

    static string[] _BlackListedUserProperties;
    static string[] BlackListedUserProperties
    {
        get
        {
            if (_BlackListedUserProperties == null)
            {
                var claimsPrincipalProps = typeof(ClaimsPrincipal).GetProperties();
                var genericPrincipalProps = typeof(GenericPrincipal).GetProperties();

                var fallbackPrincipalProps = typeof(FallbackPrincipal).GetProperties();

                var appUserProps = typeof(ApplicationUser).GetProperties();

                var principalProps = claimsPrincipalProps.Select(x => x.Name)
                    .Union(genericPrincipalProps.Select(x => x.Name))
                    .Union(appUserProps.Select(x => x.Name))
                    .Union(fallbackPrincipalProps.Select(x => x.Name))
                    .Concat(BlackListedCustomProperties)
                    .Distinct()
                    .ToList();

                _BlackListedUserProperties = principalProps.ToArray();
            }

            return _BlackListedUserProperties;
        }
    }

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
                        "IModifiedTrackable.IsModified",
                        "ParentLinkReferenceProperty",
                        "ParentLinkReference",
                        "ShouldBeImplicitlyExported",
                        "MixinInstance",
                        "Guid",
                        "ContentReference",
                        "ContentTypeID",
                        "IsReadOnly",
                        "Property",
                        "ViewData",
                        "ReferencedPermanentLinkIds"
                    })
                    .Concat(BlackListedCustomProperties)
                    .Distinct()
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

        // OPTIMIZE: Store only properties that can be read, that matches the name filter and does not contain JsonIgnore
        // Store then PropertyInfo[] in a Dictionary based on Type.GetHashCode
        // Do note: careful about anonmous types (inline "additional props"), in those types, just return GetProperties() ?
        // as theres no jsonIgnore in those props, and they can be read, and filtering by name is skipped as its EXPLICIT set as returning, written by the consumer
        var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);

        if (properties == null || properties.Length == 0) return new ExpandoObject();

        var isContentDataType = type.Inherits(Globals.ContentDataType);

        foreach (var property in properties)
        {
            var name = property.Name;

            if (!IsPropertyElligibleAsProp(isContentDataType, property, name, ignorePropertyNames))
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
                {
                    result.Add(name, null);
                }
            }

            else if (value is ContentArea contentArea)
            {
                result.Add(name, contentArea.Render());
            }

            else if (value is XhtmlString xHtmlString)
            {
                result.Add(name, xHtmlString.Render());
            }
            else if (value is string str)
                result.Add(name, str);

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
                try
                {
                    var genericType = iList.GetType().GetFirstGenericType();
                    if (genericType?.Inherits(Globals.ContentDataType) == true)
                    {
                        var listItems = GetLoopableContentDataAsDictionary(model, forceCamelCase, printNullValues, ignorePropertyNames, value, iList, genericType);

                        result.Add(name, listItems);
                    }
                    else
                    {
                        result.Add(name, iList);
                    }
                }
                catch (Exception ex)
                {
                    result.Add(name, null);
                    result.Add(name + "Error", ex.Message);
                }
            }
            else if (value is IEnumerable enumerable)
            {
                var enumerableType = enumerable.GetType();

                var genericType = enumerableType.GetFirstGenericType();

                if (genericType?.Inherits(Globals.ContentDataType) == true)
                {
                    try
                    {
                        var enumerableItems = GetLoopableContentDataAsDictionary(model, forceCamelCase, printNullValues, ignorePropertyNames, value, enumerable, genericType);

                        result.Add(name, enumerableItems);
                    }
                    catch (Exception ex)
                    {
                        result.Add(name, null);
                        result.Add(name + "Error", ex.Message);
                    }
                }
                else
                {
                    if (enumerableType.IsClass &&
                        !enumerableType.IsInterface &&
                        !enumerableType.IsArray &&
                        !enumerableType.IsGenericType &&
                        !enumerableType.IsGenericParameter &&
                        !enumerableType.IsInterface &&
                        !enumerableType.IsListOrArray() &&
                        !enumerableType.IsDictionary() &&
                        !enumerableType.IsGenericTypeDefinition)
                    {
                        try
                        {
                            var enumerableList = new List<object>();

                            foreach (var enumerableItem in enumerable)
                            {
                                enumerableList.Add(enumerableItem);
                            }
                            result.Add(name, enumerableList);
                        }
                        catch (Exception ex)
                        {
                            result.Add(name, null);
                            result.Add(name + "Error", ex.Message);
                        }
                    }
                    else
                    {
                        result.Add(name, enumerable);
                    }
                }
            }
            else if (value is Enum en)
                result.Add(name, en.ToValue());

            else if (value is MediaData mediaData)
                result.Add(name, mediaData.ContentLink?.ToFriendlyUrl());

            else
            {
                if (value is IPrincipal || value is IdentityUser)
                {
                    // OPTIMIZE: Store only properties that can be read, that matches the name filter and does not contain JsonIgnore
                    var userProperties = IdentityUserProperties.TryGet<PropertyInfo[]>(value.GetType().GetHashCode(), () =>
                    {
                        return value.GetType()?.GetProperties();
                    });

                    if (userProperties?.Length > 0)
                    {
                        var userDictionary = new Dictionary<string, object>();

                        foreach (var userProperty in userProperties)
                        {
                            if (!userProperty.CanRead) continue;

                            var userPropertyName = userProperty.Name;

                            if (userPropertyName == "Property") continue;

                            if (userPropertyName.StartsWith("EPiServer")) continue;

                            if (userPropertyName.StartsWith("EPi_")) continue;

                            if (WhiteListedCustomProperties.Contains(userPropertyName)) continue;

                            var userPropertyType = userProperty.PropertyType;

                            if (userPropertyType == SystemType) continue;

                            if (BlackListedUserProperties.Contains(userPropertyName)) continue;

                            if (userProperty.GetCustomAttribute<JsonIgnoreAttribute>() != null) continue;

                            try
                            {
                                var userPropertyValue = userProperty.GetValue(value);

                                userDictionary.Add(userPropertyName, userPropertyValue);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);

                                userDictionary.Add(userPropertyName, "errored");
                            }
                        }

                        result.Add(name, userDictionary);
                    }
                }
                else
                {
                    result.Add(name, value);
                }
            }
        }

        return result;
    }

    static List<IDictionary<string, object>> GetLoopableContentDataAsDictionary(object model, bool forceCamelCase, bool printNullValues, string[] ignorePropertyNames, object value, IEnumerable list, Type genericType)
    {
        var hashCode = genericType.GetHashCode();
        var properties = ReactPropPropertiesCache.TryGet(hashCode, () =>
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

                object listPropValue = null;

                try
                {
                    listPropValue = listProp.GetValue(content);
                }
                catch
                {
                    // Swallow
                    continue;
                }

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

    static bool IsPropertyElligibleAsProp(bool isContentDataType, PropertyInfo property, string name, string[] ignorePropertyNames)
    {
        if (!property.CanRead) return false;

        if (name == "Property") return false;

        if (name.StartsWith("EPiServer.")) return false;

        if (name.StartsWith("EPi_")) return false;

        if (WhiteListedCustomProperties.Contains(name)) return true;

        if (isContentDataType && BlackListedContentProperties.Contains(name)) return false;

        if (ignorePropertyNames != null)
        {
            if (ignorePropertyNames.Contains(name))
            {
                return false;
            }
        }

        var propertyType = property.PropertyType;

        if (propertyType.IsClass && (name.StartsWith("CurrentBlock") || name.StartsWith("CurrentPage") || name.StartsWith("CurrentMedia"))) return false;

        if (propertyType == MessageType ||
            propertyType == ParentLinkReferenceType ||
            propertyType == SystemType ||
            propertyType == CultureInfoType ||
            propertyType == PropertyUrlType)
            return false;

        if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null) return false;

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
}