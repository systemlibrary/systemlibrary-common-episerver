using System.Collections.Concurrent;
using System.Reflection;

using SystemLibrary.Common.Framework.Extensions;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static ConcurrentDictionary<int, PropertyInfo[]> ModelToPropsPropertiesCache = new ConcurrentDictionary<int, PropertyInfo[]>();

    static IDictionary<string, object> ModelToProps(Type modelType, object model, object additionalProps, bool forceCamelCase, bool printNullValues)
    {
        List<string> ignorePropertyNames = null;

        if (additionalProps != null)
        {
            var additionalType = additionalProps.GetType();

            int hashCode;

            unchecked
            {
                hashCode = additionalType.GetHashCode() ^ modelType.Name.GetHashCode();
            }

            // NOTE: Optimize: return the ignorePropertyNames as array directly, instead of looping afterwards
            var additionalProperties = ModelToPropsPropertiesCache.Cache(hashCode, () =>
            {
                return additionalType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.GetProperty);
            });

            if (additionalProperties != null && additionalProperties.Length > 0)
            {
                ignorePropertyNames = new List<string>();

                foreach (var additionalPropertyName in additionalProperties)
                    ignorePropertyNames.Add(additionalPropertyName.Name);
            }
        }

        var props = model.ToPropsDictionary(forceCamelCase, printNullValues, ignorePropertyNames?.ToArray());

        if (additionalProps != null)
        {
            IDictionary<string, object> additional = additionalProps.ToPropsDictionary(forceCamelCase, printNullValues);

            if (additional != null && additional.Count > 0)
            {
                foreach (var kv in additional)
                    props.Add(kv.Key, kv.Value);
            }
        }

        return props;
    }
}