using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Components;

/// <summary>
/// Block Components without a specific 'AsyncComponent' will automatically inherit and use this
/// <para>This assumes by convention, that there is then a 'Index.cshtml' or 'BlockName.cshtml' file located at the 'DefaultComponentPathPredicate' for the specific Block Type</para>
/// Or; in other words-ish: In the same folder next to the "BlockData.cs" file, there should be a view file
/// </summary>
/// <remarks>
/// Important to note that this is only in use if you do not bother creating your own Component.cs file (previously named DefaultBlockController or similar)
/// </remarks>
/// <example>
/// Components/Car/CarBlock.cs
/// <code>
/// CarBlock : BlockData { }
/// </code>
/// Components/Car/Index.cshtml
/// <code>
/// @model object
/// Hello world from Car Index.cshtml
/// </code>
/// <code>
/// options.DefaultComponentPathPredicate = (Type blockType) =>
/// {
///     // Can return any path (folder), and also vary based on the Type if you want
///     return "~/Components/";
/// };
/// </code>
/// CarBlock will use the Default Component Path Predicate, and return "~/Components/" as the root folder
/// This means that Car/Index.cshtml will be found inside the /Components/ folder, and the view is rendered with the Car data
/// Both Index.cshtml and Car.cshtml file name conventions for the view would work out of the box
/// </example>
[TemplateDescriptor(Inherited = true)]
public class _DefaultBlockComponent : AsyncComponent<BlockData>
{
    // TODO: Consider if theres no view file found, for this component, but a BlockData, and this DefaultComponent has been invoked
    // should we then assume the consumer wants to use our ReactServerSideRendering with its default params?
    // In theory consumers then only create a Block.cs and then a React.jsx file and voila!
    internal static Func<Type, string> DefaultComponentPathPredicate = null;
    protected override async Task<IViewComponentResult> InvokeComponentAsync(BlockData currentBlock)
    {
        var type = currentBlock.GetOriginalType();

        if (DefaultComponentPathPredicate == null)
            throw new Exception("DefaultComponentPathPredicate is null and we did not find a View for current block data. Register DefaultComponentPathPredicate and the return path must start with ~/ and end with /. It must point to the parent folder of where your block " + type.Name + " lives. Example: ~/Buttons/ButtonBlock/Index.cshtml, then '~/Buttons/' should be returned, as the Type/Index.cshtml is always inferred. Set the predicate method in the options parameter you pass to: app.UseCommonCmsApp(CommonEpiserverAppOptions)");

        var componentStartingPath = DefaultComponentPathPredicate(type);

        if (!(componentStartingPath[0] == '~') || !componentStartingPath.EndsWith("/", StringComparison.Ordinal))
            throw new Exception("DefaultComponentPathPredicate must start with ~/ and end with /. It must point to the parent folder of where your block " + type.Name + " lives. Example: ~/Buttons/ButtonBlock/Index.cshtml, then '~/Buttons/' should be returned, as the Type/Index.cshtml and Type/Type.cshtml is always checked");

        var viewPath = string.Format(componentStartingPath + "{0}/Index.cshtml", type.Name);

        var view = ViewEngine.GetView(null, viewPath, false);

        if (!view.Success)
            viewPath = string.Format(componentStartingPath + "{0}/{1}.cshtml", type.Name, type.Name);

        return await Task.FromResult(View(viewPath, currentBlock));
    }
}