using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Cms;

public class ReactResult : ContentResult
{
    public ReactResult(object model, object additionalProps = null, string tag = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null)
    {
        ContentType = "text/html";
        Content = model.ServerSideRenderReactComponent(additionalProps, tag, camelCaseProps, cssClass, id, componentFullName);
    }
}
