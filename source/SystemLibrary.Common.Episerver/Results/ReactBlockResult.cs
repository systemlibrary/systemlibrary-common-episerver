using System.Text;

using EPiServer;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver;

public class ReactBlockResult : ContentResult
{
    public ReactBlockResult(object model, object additionalProps = null, bool camelCaseProps = false, bool renderClientOnly = false, bool renderServerOnly = false, string tagName = "div", string cssClass = null, string id = null, string componentFullName = null, bool printNullValues = true)
    {
        ContentType = "text/html";

        // TODO: What is this? Looks like "GetComponentFullName", but it is the old version, instead of Component, its Block, so this will be deleted in 12-24 months
        if (componentFullName.IsNot())
        {
            var name = model?.GetOriginalType()?.Name ?? "";

            if (name.Length > 0 && name[0] == '<')
                componentFullName = name.Replace("<>", "").Replace("`", "").Replace(" ", "");

            else if (name != "ViewModel" && name.EndsWith("ViewModel", StringComparison.Ordinal))
                componentFullName = "reactBlocks." + name.Substring(0, name.Length - "ViewModel".Length);

            else if (name != "Model" && name.EndsWith("Model", StringComparison.Ordinal))
                componentFullName = "reactBlocks." + name.Substring(0, name.Length - "Model".Length);
            else if (name.EndsWith("Block"))
                componentFullName = "reactBlocks." + name;
        }
        Content = model.ReactServerSideRender(additionalProps, tagName, camelCaseProps, cssClass, id, componentFullName, renderClientOnly, renderServerOnly, printNullValues).ToString();
    }
}