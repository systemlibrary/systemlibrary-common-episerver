using System.Collections;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json.Serialization;

using EPiServer;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;
using EPiServer.Security.Internal;
using EPiServer.Shell.Web;
using EPiServer.SpecializedProperties;

using Microsoft.AspNetCore.Identity;

using SystemLibrary.Common.Episerver.Properties;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Object extensions
/// </summary>
public static class ObjectExtensions
{
    static Type SystemType;
    static Type MessageType;
    static Type ParentLinkReferenceType;
    static Type CultureInfoType;
    static Type PropertyUrlType;

    static ObjectExtensions()
    {
        SystemType = typeof(Type);
        MessageType = typeof(Message);
        ParentLinkReferenceType = typeof(ParentLinkReference);
        CultureInfoType = typeof(CultureInfo);
        PropertyUrlType = typeof(PropertyUrl);
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

    static ConcurrentDictionary<int, PropertyInfo[]> GetPublicInstancePropertiesCache = new ConcurrentDictionary<int, PropertyInfo[]>();

    static bool IsPropertyElligibleAsPropData(PropertyInfo property, bool isModelContentDataType, string[] ignorePropertyNames)
    {
        if (!property.CanRead) return false;

        var name = property.Name;

        if (name == "Property") return false;

        if (name.StartsWith("EPiServer.", StringComparison.Ordinal)) return false;

        if (name.StartsWith("EPi_", StringComparison.Ordinal)) return false;

        if (WhiteListedCustomProperties.Contains(name)) return true;

        if (isModelContentDataType && BlackListedContentProperties.Contains(name)) return false;

        if (ignorePropertyNames != null)
        {
            if (ignorePropertyNames.Contains(name))
            {
                return false;
            }
        }

        var propertyType = property.PropertyType;

        if (propertyType.IsClass && (name.StartsWith("CurrentBlock", StringComparison.Ordinal) || name.StartsWith("CurrentPage", StringComparison.Ordinal) || name.StartsWith("CurrentMedia", StringComparison.Ordinal))) return false;

        if (propertyType == MessageType ||
            propertyType == ParentLinkReferenceType ||
            propertyType == SystemType ||
            propertyType == CultureInfoType ||
            propertyType == PropertyUrlType)
            return false;

        if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null) return false;

        return true;

    }

    static PropertyInfo[] GetPublicInstanceProperties(Type type, bool isModelContentDataType, string[] ignorePropertyNames)
    {
        return GetPublicInstancePropertiesCache.Cache(type.GetHashCode(), () =>
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            var cache = new List<PropertyInfo>();

            foreach (var property in properties)
            {
                if (IsPropertyElligibleAsPropData(property, isModelContentDataType, ignorePropertyNames))
                    cache.Add(property);
            }

            return cache.ToArray();
        });
    }

    /// <summary>
    /// Convert a model to a Dictionary, which is ready to be used as Props to a React Component
    /// <para>For example it will render ContentAreas, or IList with various Episerver types, and the outputted rendered HTML of those types will be added as a Prop (name) in the Dictionary</para>
    /// </summary>
    /// <param name="model">A class with properties where each property will be converted, rendered, and validated, and then added as a Prop</param>
    /// <param name="forceCamelCase">True or false, based on how you write your props in React. "firstName" or "FirstName"</param>
    /// <param name="printNullValues">True or false, based on wether you want to pass data to the DOM, even if "firstName=null", or you want to skip it completely, possibly ending up as "undefined" in the Frontend World</param>
    /// <param name="ignorePropertyNames">Certain properties you do not want to send to your React (frontend), for instance a Token, Secret? Instead of generating a new model and map over, simply add the name of the properties here</param>
    /// <returns>A dictionary ready to be used as Props in React World</returns>
    public static IDictionary<string, object> ToReactPropsDictionary(this object model, bool forceCamelCase = false, bool printNullValues = true, params string[] ignorePropertyNames)
    {
        // TODO: Rename "ToPropsOrDataDictionary", converting and rendering most data in the Model, to a Dictionary, which can be used to Serialize/Whatever, manipulate the data, keys,... before sending to React/Whatever ...
        if (model == null) return new Dictionary<string, object>();

        var type = model.GetType();

        if (!type.IsClass || type.IsInterface) throw new Exception("Cannot pass a non-class as model for react properties. Either create a class or an anonymous object and pass that with the variables you want as properties in your react component");

        var isContentDataType = type.Inherits(Globals.ContentDataType);

        var properties = GetPublicInstanceProperties(type, isContentDataType, ignorePropertyNames);

        if (properties.Length == 0) return new ExpandoObject();

        Dump.Write(type.Name);

        var result = new Dictionary<string, object>();

        foreach (var property in properties)
        {
            ConvertPropertyToDictionaryData(model, type, property, result, forceCamelCase, printNullValues, ignorePropertyNames);
        }

        return result;
    }

    static void ConvertPropertyToDictionaryData(object model, Type modelType, PropertyInfo property, Dictionary<string, object> result, bool forceCamelCase, bool printNullValues, string[] ignorePropertyNames)
    {
        var name = property.Name;

        object value;

        try
        {
            value = property.GetValue(model);
        }
        catch
        {
            Debug.Log("Swallow: could not get value of property " + name + " on type " + modelType.Name);
            return;
        }


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
            result.Add(name, contentArea.Render());

        else if (value is XhtmlString xHtmlString)
            result.Add(name, xHtmlString.Render());

        else if (value is string str)
            result.Add(name, str);

        else if (value is Url url)
            result.Add(name, url.ToFriendlyUrl());

        else if (value is Uri uri)
            result.Add(name, uri.ToFriendlyUrl());

        else if (value is LinkItem linkItem)
            result.Add(name, GetAttributesOfLinkItem(linkItem));

        else if (value is IList<LinkItem> linkItems)
        {
            var linkItemAttributes = linkItems.Where(x => x.Attributes != null)?.Select(x => GetAttributesOfLinkItem(x));

            result.Add(name, linkItemAttributes);
        }
        else if (value is ContentReference contentReference)
            result.Add(name, contentReference.ToFriendlyUrl());

        else if (value is PageData page)
            result.Add(name, page.ToFriendlyUrl());

        else if (value is MediaData media)
            result.Add(name, media.ContentLink.ToFriendlyUrl());

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
                Log.Error("Converting " + name + " to prop data failed: " + ex.Message);
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
                    Log.Error("Converting " + name + " to prop data failed: " + ex.Message);
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
                        Log.Error("Converting " + name + " to prop data failed: " + ex.Message);
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
            result.Add(name, mediaData?.ContentLink.ToFriendlyUrl());

        else
        {
            if (value is IPrincipal || value is IdentityUser)
            {
                var userProperties = GetPublicInstanceProperties(value.GetType(), true, ignorePropertyNames);

                if (userProperties.Length == 0) return;

                var userDictionary = new Dictionary<string, object>();

                foreach (var userProperty in userProperties)
                {
                    var userPropertyName = userProperty.Name;

                    if (forceCamelCase)
                    {
                        if (userPropertyName.Length <= 1)
                            userPropertyName = userPropertyName.ToLower();
                        else
                            userPropertyName = char.ToLowerInvariant(userPropertyName[0]) + userPropertyName.Substring(1);
                    }

                    try
                    {
                        userDictionary.Add(userPropertyName, userProperty.GetValue(value));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Converting " + userPropertyName + " to prop data failed: " + ex.Message);
                        userDictionary.Add(userPropertyName, null);
                        userDictionary.Add(userPropertyName + "Error", ex.Message);
                    }
                }
                result.Add(name, userDictionary);
            }
            else
            {
                result.Add(name, value);
            }
        }
    }

    static List<IDictionary<string, object>> GetLoopableContentDataAsDictionary(object model, bool forceCamelCase, bool printNullValues, string[] ignorePropertyNames, object value, IEnumerable contentList, Type genericType)
    {
        var properties = GetPublicInstanceProperties(genericType, isModelContentDataType: true, ignorePropertyNames);

        var result = new List<IDictionary<string, object>>();

        if (properties.Length == 0) return result;

        foreach (var contentData in contentList)
        {
            if (contentData == model) continue;

            var data = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                ConvertPropertyToDictionaryData(model, genericType, property, data, forceCamelCase, printNullValues, ignorePropertyNames);
            }

            result.Add(data);
        }

        return result;
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