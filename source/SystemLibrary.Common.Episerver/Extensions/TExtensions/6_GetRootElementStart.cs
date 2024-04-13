using System.Text;
using System.Text.RegularExpressions;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static string GetComponentContainerClassName(string componentFullName)
    {
        var index = componentFullName.LastIndexOf('.');

        if (index == -1) return componentFullName.ToLower() + "-container";

        var componentName = componentFullName.Substring(index + 1);

        var temp = Regex.Replace(componentName, "([A-Z])", "-$1");

        if (temp[0] == '-')
            return temp.Substring(1).ToLower() + "-container";

        return temp.ToLower() + "-container";
    }

    static StringBuilder GetRootElementStart(string componentFullName, string id, string cssClass, string tagName, string ssrId)
    {
        if (tagName != null)
        {
            StringBuilder root;

            if (id.Is())
            {
                root = new StringBuilder("<" + tagName + " id=\"" + id + "\"", 1024);
            }
            else
            {
                root = new StringBuilder("<" + tagName, 1024);
            }

            if (cssClass == "")
                root.Append(" class=\"" + GetComponentContainerClassName(componentFullName) + "\"");

            else if (cssClass != null)
                root.Append($" class=\"{cssClass}\"");

            if (ssrId != null)
            {
                root.Append(" data-rcssr-id=\"" + ssrId + "\"");
            }

            root.Append(">");

            return root;
        }
        else
        {
            return new StringBuilder(1024);
        }
    }
}