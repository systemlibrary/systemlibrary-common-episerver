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
    internal static Func<string, string> DefaultComponentPathPredicate = null;
    protected override async Task<IViewComponentResult> InvokeComponentAsync(BlockData currentBlock)
    {
        var type = currentBlock.GetOriginalType();

        if (DefaultComponentPathPredicate == null)
            throw new Exception(type.Name + " is missing a 'controller/component', tried using 'SystemLibrary.Common.Episerver.DefaultComponent', but DefaultComponentPathPredicate is null. Please specify it in the options object when you invoke: app.CommonEpiserverApp(CommonEpiserverAppOptions) options. It should be the default root folder where block/components lives in, for instance ~/Content/Blocks/");

        var componentStartingPath = DefaultComponentPathPredicate(type.Name);

        if (!componentStartingPath.StartsWith("~") || !componentStartingPath.EndsWith("/"))
            throw new Exception("DefaultComponentPathPredicate must start with ~/ and end with /. It must point to the parent folder of where your block " + type.Name + " lives. Example: ~/Button/ButtonBlock.cshtml, then '~/Blocks/' should be returned");

        var viewPath = string.Format(componentStartingPath + "{0}/{1}.cshtml", type.Name, type.Name);

        var view = ViewEngine.GetView(null, viewPath, false);

        if (!view.Success)
            viewPath = string.Format(componentStartingPath + "{0}/{1}.cshtml", type.Name, "Index");

        return await Task.FromResult(View(viewPath, currentBlock));
    }
}