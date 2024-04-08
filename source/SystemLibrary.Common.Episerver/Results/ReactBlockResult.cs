using System.Text;

using EPiServer;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver;

public class ReactBlockResult : ContentResult
{
    public ReactBlockResult(object model, object additionalProps = null, bool camelCaseProps = false, bool renderClientOnly = false, bool renderServerOnly = false, string tagName = "div", string cssClass = null, string id = null, string componentFullName = null)
    {
        ContentType = "text/html";

        Content = model.ReactServerSideRender(additionalProps, tagName, camelCaseProps, cssClass, id, componentFullName, renderClientOnly, renderServerOnly).ToString();
    }
}