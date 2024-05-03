using System.Text;
using System.Text.RegularExpressions;

using EPiServer;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string GetComponentFullName(Type modelType, object model, string componentFullName)
    {
        if(componentFullName != null) return componentFullName;

        var name = modelType.Name;

        if(name.StartsWith("<"))
            name = name.Replace("<>", "").Replace("`", "").Replace(" ", "");

        if (name != "ViewModel" && name.EndsWith("ViewModel"))
            return "reactComponents." + name.Substring(0, name.Length - "ViewModel".Length);

        if (name != "Model" && name.EndsWith("Model"))
            return "reactComponents." + name.Substring(0, name.Length - "Model".Length);

        return "reactComponents." + name;
    }
}