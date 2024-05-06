using System.Dynamic;
using System.Reflection;
using System.Text;

using Org.BouncyCastle.Bcpg.OpenPgp;

using SystemLibrary.Common.Net.Extensions;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static IDictionary<string, object> ModelToProps(object model, object additionalProps, bool forceCamelCase, bool printNullValues)
    {
        List<string> ignorePropertyNames = null;

        if (additionalProps != null)
        {
            // NOTE: Additional props hashCode should including current Model.Type hashCode
            // converted to a BigInt
            var additionalType = additionalProps.GetType();

            var hashCode = additionalType.GetHashCode();

            var additionalProperties = Dictionaries.ModelToPropsPropertiesCache.TryGet(hashCode, () =>
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

        var props = model.ToReactPropsDictionary(forceCamelCase, printNullValues, ignorePropertyNames?.ToArray());

        if (additionalProps != null)
        {
            IDictionary<string, object> additional = additionalProps.ToReactPropsDictionary(forceCamelCase);

            if (additional != null && additional.Count > 0)
            {
                foreach (var kv in additional)
                    props.Add(kv.Key, kv.Value);
            }
        }

        return props;
    }
}