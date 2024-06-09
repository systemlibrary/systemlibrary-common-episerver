using System;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Components;

[TemplateDescriptor(Inherited = true)]
public class DefaultBlockComponent : AsyncComponent<BlockData>
{
    internal static Func<Type, string> DefaultComponentPathPredicate = null;
    protected override async Task<IViewComponentResult> InvokeComponentAsync(BlockData currentBlock)
    {
        var type = currentBlock.GetOriginalType();

        if (DefaultComponentPathPredicate == null)
            throw new Exception("DefaultComponentPathPredicate is null. It should be registered and result must start with ~/ and end with /. It must point to the parent folder of where your block " + type.Name + " lives. Example: ~/Buttons/ButtonBlock/Index.cshtml, then '~/Buttons/' should be returned, as the Type/Index.cshtml is always appended. Set the predicate method in the options parameter you pass to: app.UseCommonCmsApp(CommonEpiserverAppOptions)");

        var componentStartingPath = DefaultComponentPathPredicate(type);

        if (!componentStartingPath.StartsWith("~") || !componentStartingPath.EndsWith("/"))
            throw new Exception("DefaultComponentPathPredicate must start with ~/ and end with /. It must point to the parent folder of where your block " + type.Name + " lives. Example: ~/Buttons/ButtonBlock/Index.cshtml, then '~/Buttons/' should be returned, as the Type/Index.cshtml and Type/Type.cshtml is always checked");

        var viewPath = string.Format(componentStartingPath + "{0}/Index.cshtml", type.Name);

        var view = ViewEngine.GetView(null, viewPath, false);

        if (!view.Success)
            viewPath = string.Format(componentStartingPath + "{0}/{1}.cshtml", type.Name, type.Name);

        return await Task.FromResult(View(viewPath, currentBlock));
    }
}