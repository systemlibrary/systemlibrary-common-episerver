using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver;

// Calls the initial render method, properties are not ready, but all properties are set on the html element as a json string which is now ready to be parsed and Hydrated by the react function React.Hydrate(element);
public class ReactComponentResult : BaseReactComponentResult
{
    public ReactComponentResult(object model, object additionalProps = null, bool camelCaseProps = false, bool renderClientOnly = false, bool renderServerOnly = false, string tagName = "div", string cssClass = null, string id = null, string componentFullName = null)
    {
        var type = GetType(model);

        componentFullName = GetReactComponentFullName(type, componentFullName);

        ContentType = "text/html";

        var data = model.ReactServerSideRender(type, additionalProps, tagName, camelCaseProps, cssClass, id, componentFullName, renderClientOnly, renderServerOnly);

        if (!renderServerOnly)
            AppendClientProperties(data);

        //Dump.Write("RENDERED: " + type.Name);
        //Dump.Write(data.ToString());

        Content = data.ToString();
    }
}