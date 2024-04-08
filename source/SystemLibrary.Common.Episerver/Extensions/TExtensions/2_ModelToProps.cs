using System.Dynamic;
using System.Text;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static ExpandoObject ModelToProps(object model, object additionalProps, bool forceCamelCase)
    {
        IDictionary<string, object> props = model.ToExpandoObject(forceCamelCase);

        var propCount = props.Count();

        for (int i = 0; i < propCount; i++)
        {
            var property = props.ElementAt(i);

            if (property.Value == null)
                continue;

            else if (property.Value is StringBuilder sb)
                props[property.Key] = sb?.ToString();
        }

        IDictionary<string, object> additional = additionalProps.ToExpandoObject(forceCamelCase);

        if (additional != null && additional.Count > 0)
        {
            foreach (var kv in additional)
            {
                if (props.ContainsKey(kv.Key))
                    props.Remove(kv.Key);

                props.Add(kv.Key, kv.Value);
            }
        }
        return (ExpandoObject)props;
    }
}