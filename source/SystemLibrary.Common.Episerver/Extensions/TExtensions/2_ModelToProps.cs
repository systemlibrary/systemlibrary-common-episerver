using System.Dynamic;
using System.Text;

using Org.BouncyCastle.Bcpg.OpenPgp;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static IDictionary<string, object> ModelToProps(object model, object additionalProps, bool forceCamelCase, bool printNullValues)
    {
        List<string> ignorePropertyNames = null;

        if (additionalProps != null)
        {
            var additionalType = additionalProps.GetType();

            var additionalProperties = additionalType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);

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