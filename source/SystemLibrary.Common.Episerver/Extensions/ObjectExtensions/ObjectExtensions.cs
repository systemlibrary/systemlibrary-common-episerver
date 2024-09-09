using System.Collections;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json.Serialization;

using Castle.Core.Internal;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;
using EPiServer.Security.Internal;
using EPiServer.Shell.Web;
using EPiServer.SpecializedProperties;

using Microsoft.AspNetCore.Identity;

using SystemLibrary.Common.Episerver.Attributes;
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
        "PhoneNumber",
        "IsAuthenticated"
    };

    static string[] ManualBlackListedContentProperties = new string[]
    {
        "PrimaryIdentitySelector",
        "ClaimsPrincipalSelector",
        "IsApproved",
        "CurrentPage",
        "CurrentBlock",
        "Password",
        "PasswordHash",
        "PasswordQuestion",
        "Properties",
        "SecurityStamp",
        "ConcurrencyStamp",
        "TwoFactorEnabled",
        "AnonymousPrincipal",
        "LastLoginDate",
        "IdToken",
        "IdTokenHint",
        "PhoneNumberConfirmed",
        "AccessFailedCount",
        "ProviderName",
        "IsAdministrator"
    };

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
                    .Concat(ManualBlackListedContentProperties)
                    .Distinct()
                    .ToArray();
            }

            return _BlackListedContentProperties;
        }
    }

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
                    .Concat(ManualBlackListedContentProperties)
                    .Distinct()
                    .ToList();

                foreach (var whitelisted in WhiteListedCustomProperties)
                    principalProps.Remove(whitelisted);

                principalProps.Add("Id");
                principalProps.Add("AuthenticationType");

                _BlackListedUserProperties = principalProps.ToArray();
            }

            return _BlackListedUserProperties;
        }
    }

    static ConcurrentDictionary<int, PropertyInfo[]> GetPublicInstancePropertiesCache = new();
    static ConcurrentDictionary<int, PropertyInfo[]> GetPublicIdentityPropertiesCache = new();

    static bool IsPropertyElligibleAsPropData(PropertyInfo property, bool isModelContentDataType)
    {
        if (!property.CanRead) return false;

        var name = property.Name;

        if (name == "Property") return false;

        if (name.StartsWith("EPiServer.", StringComparison.Ordinal)) return false;

        if (name.StartsWith("EPi_", StringComparison.Ordinal)) return false;

        if (WhiteListedCustomProperties.Contains(name)) return true;

        if (isModelContentDataType && BlackListedContentProperties.Contains(name)) return false;

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

    static PropertyInfo[] GetPublicInstanceProperties(Type type, bool isModelContentDataType)
    {
        return GetPublicInstancePropertiesCache.Cache(type.GetHashCode(), () =>
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            var cache = new List<PropertyInfo>();

            foreach (var property in properties)
            {
                if (IsPropertyElligibleAsPropData(property, isModelContentDataType))
                    cache.Add(property);
            }

            return cache.ToArray();
        });
    }

    static PropertyInfo[] GetPublicIdentityProperties(Type type)
    {
        return GetPublicIdentityPropertiesCache.Cache(type.GetHashCode(), () =>
        {
            var properties = GetPublicInstanceProperties(type, false);

            var cache = new List<PropertyInfo>();

            foreach (var property in properties)
            {
                if (BlackListedUserProperties.Contains(property.Name)) continue;

                cache.Add(property);
            }

            return cache.ToArray();
        });
    }

    internal static IDictionary<string, object> ToPropsDictionary(this object model, bool forceCamelCase = false, bool printNullValues = true, params string[] ignorePropertyNames)
    {
        // TODO: Name could be 'ToDictionary' as that's all it does, converting most data in to a decent dictionary format
        if (model == null) return new Dictionary<string, object>();

        var type = model.GetType();

        if (!type.IsClass || type.IsInterface) throw new Exception("Cannot pass a non-class as model for react properties. Either create a class or an anonymous object and pass that with the variables you want as properties in your react component");

        var isContentDataType = type.Inherits(Globals.ContentDataType);

        var properties = GetPublicInstanceProperties(type, isContentDataType);

        if (properties.Length == 0) return new ExpandoObject();

        var result = new Dictionary<string, object>();

        const int level = 0;
        foreach (var property in properties)
        {
            ConvertPropertyToDictionaryData(model, type, property, result, forceCamelCase, printNullValues, ignorePropertyNames, false, level);
        }

        return result;
    }

    static void ConvertPropertyToDictionaryData(object model, Type modelType, PropertyInfo property, Dictionary<string, object> result, bool forceCamelCase, bool printNullValues, string[] ignorePropertyNames, bool convertContentAreaToList, int level)
    {
        if (level > 12)
        {
            Log.Warning("Skipping conversion to dictionary data, as we are deper than level 12 in the recursive method, for type: " + modelType.Name);
            return;
        }

        var name = property.Name;

        if (ignorePropertyNames != null)
        {
            if (ignorePropertyNames.Contains(name))
            {
                Debug.Log("Skipped property " + name);
                return;
            }
        }

        object value;

        try
        {
            value = property.GetValue(model);
        }
        catch
        {
            Log.Warning("Could not get value of property " + name + " on type " + modelType.Name + " " + model.GetType().Name + " " + property.DeclaringType?.Name + " (" + property.PropertyType.Name + ")");
            return;
        }

        if (forceCamelCase)
        {
            if (name.Length <= 1)
                name = name.ToLower();
            else
                name = char.ToLowerInvariant(name[0]) + name[1..];
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
            if (convertContentAreaToList)
            {
                var forceContentAreaRenderAsString = property.GetCustomAttribute<ServerSideRenderStringAttribute>();
                if (forceContentAreaRenderAsString == null)
                {
                    var contentData = contentArea.SelectFiltered<ContentData>();

                    if (contentData.Is())
                    {
                        var items = GetLoopableContentDataAsDictionary(contentData, forceCamelCase, printNullValues, ignorePropertyNames, model, level);

                        result.Add(name, items);
                    }
                    else
                    {
                        if (printNullValues)
                        {
                            result.Add(name, null);
                        }
                    }
                }
                else
                {
                    Debug.Log("Property " + name + " is forced rendered as string due to attribute");

                    result.Add(name, contentArea.Render());
                }
            }
            else
            {
                result.Add(name, contentArea.Render());
            }
        }

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

        else if (value is IEnumerable enumerable)
        {
            var enumerableType = enumerable.GetType();

            var genericType = enumerableType.GetFirstGenericType();

            if (genericType == Globals.ContentReferenceType || genericType.Inherits(Globals.ContentReferenceType))
            {
                var list = new List<object>();

                foreach (var contentRef in enumerable)
                {
                    if (contentRef == null) continue;

                    if(contentRef is ContentReference cr)
                    {
                        var crFriendlyUrl = cr.ToFriendlyUrl();

                        if(crFriendlyUrl.EndsWith(".jpg"))
                            crFriendlyUrl = crFriendlyUrl + "?quality=80";

                        var icontentData = cr.To<IContent>();

                        var linkName = icontentData?.Name;

                        if (forceCamelCase)
                        {
                            list.Add(new
                            {
                                id = cr.ID,
                                linkName = linkName,
                                url = crFriendlyUrl
                            });
                        }
                        else
                        {
                            list.Add(new
                            {
                                Id = cr.ID,
                                LinkName = linkName,
                                Url = crFriendlyUrl
                            });
                        }
                    }
                }

                result.Add(name, list);
            }
            else if (genericType?.Inherits(Globals.ContentDataType) == true)
            {
                try
                {
                    var enumerableItems = GetLoopableContentDataAsDictionary(enumerable, forceCamelCase, printNullValues, ignorePropertyNames, model, level);

                    result.Add(name, enumerableItems);
                }
                catch (Exception ex)
                {
                    Log.Error("Converting IEnumerable " + name + " to prop data failed: " + ex.Message);
                    result.Add(name, null);
                    result.Add(name + "Error", ex.Message);
                }
            }
            else
            {
                // NOTE: Transforming dynamic IEnumerable when they yield 
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
                        Log.Error("Converting enumerable list " + name + " to prop data failed: " + ex.Message);
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
                var userProperties = GetPublicIdentityProperties(value.GetType());

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

    static List<IDictionary<string, object>> GetLoopableContentDataAsDictionary(IEnumerable contentList, bool forceCamelCase, bool printNullValues, string[] ignorePropertyNames, object model, int level)
    {
        var result = new List<IDictionary<string, object>>();

        foreach (var contentData in contentList)
        {
            if (contentData == null) continue;

            if (contentData is IContent icontent && (icontent.IsDeleted || !icontent.IsPublished())) continue;

            if (contentData == model) continue;

            var type = contentData.GetType();

            var properties = GetPublicInstanceProperties(type, true);

            if (properties.Length == 0) continue;

            var data = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                try
                {
                    ConvertPropertyToDictionaryData(contentData, type, property, data, forceCamelCase, printNullValues, ignorePropertyNames, true, level + 1);
                }
                catch (Exception ex)
                {
                    Debug.Log(type.Name + " with property " + property.Name + " could not be converted, error " + ex.Message);
                }
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