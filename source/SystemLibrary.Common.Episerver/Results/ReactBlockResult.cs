using EPiServer;

using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// React Block Result is for backward compatibility, Epi 11, if youre still using BlockControllers instead of Components
/// </summary>
/// <remarks>
/// Assumes by default that all frontend react components is registered under DOM variable "window.reactBlocks."
/// </remarks>
/// <example>
/// <code>
/// public class CarBlockController : BlockController&lt;CarBlock&gt;
/// {
///     public override ActionResult Index(CarBlock currentBlock)
///     {
///         return new ReactBlockResult(currentBlock, renderClientOnly: true);
///     }
/// }
/// </code>
/// </example>
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