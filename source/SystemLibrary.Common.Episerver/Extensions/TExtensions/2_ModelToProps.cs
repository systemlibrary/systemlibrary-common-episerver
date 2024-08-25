using System.Collections.Concurrent;
using System.Reflection;

using SystemLibrary.Common.Net.Extensions;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static ConcurrentDictionary<int, PropertyInfo[]> ModelToPropsPropertiesCache = new ConcurrentDictionary<int, PropertyInfo[]>();

    static IDictionary<string, object> ModelToProps(Type modelType, object model, object additionalProps, bool forceCamelCase, bool printNullValues)
    {
        List<string> ignorePropertyNames = null;

        if (additionalProps != null)
        {
            // NOTE: Additional props hashCode should include current Model.Type hashCode
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

        Dump.Write("To React...");

        var props = model.ToReactPropsDictionary(forceCamelCase, printNullValues, ignorePropertyNames?.ToArray());

        if (additionalProps != null)
        {
            Debug.Log("Additionall props");
            IDictionary<string, object> additional = additionalProps.ToReactPropsDictionary(forceCamelCase);
            Dump.Write(additional);

            if (additional != null && additional.Count > 0)
            {
                foreach (var kv in additional)
                    props.Add(kv.Key, kv.Value);
            }
        }

        return props;
    }
}