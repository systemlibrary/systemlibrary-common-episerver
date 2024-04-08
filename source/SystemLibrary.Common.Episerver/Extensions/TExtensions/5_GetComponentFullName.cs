using System.Text;
using System.Text.RegularExpressions;

using EPiServer;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string GetComponentFullName(object model, string componentFullName)
    {
        if(componentFullName != null) return componentFullName;

        var type = model.GetOriginalType();
    
        var name = type.Name;

        if (name.EndsWith("ViewModel"))
            return "reactComponents." + name.Substring(0, name.Length - "ViewModel".Length);

        if (name.EndsWith("Model"))
            return "reactComponents." + name.Substring(0, name.Length - "Model".Length);

        return "reactComponents." + name;
    }
}