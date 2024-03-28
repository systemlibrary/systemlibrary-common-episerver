using System;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Components;

[TemplateDescriptor(Inherited = true)]
public class DefaultBlockComponent : AsyncComponentResult<BlockData>
{
    internal static Func<string, string> DefaultBlockComponentFolderPathPredicate = null;
    protected override async Task<IViewComponentResult> InvokeComponentAsync(BlockData currentBlock)
    {
        if (DefaultBlockComponentFolderPathPredicate == null)
            throw new Exception("A block is missing a controller, tried using 'SystemLibrary.Common.Episerver.DefaultBlockComponent' as a controller, but DefaultBlockComponentFolderPathPredicate is null. Please specify it in the options object when you invoke: app.CommonEpiserverApplicationBuilder(CommonEpiserverApplicationBuilderOptions) options. It should be the default root folder where block lives in, for instance ~/Content/Blocks/");

        var blockName = currentBlock.GetOriginalType().Name;

        var blockComponentFolderPath = DefaultBlockComponentFolderPathPredicate(blockName);

        if (!blockComponentFolderPath.StartsWith("~") || !blockComponentFolderPath.EndsWith("/"))
            throw new Exception("DefaultBlockComponentFolderPath must start with ~/ and end with /. It must point to the parent folder of where your block " + blockName + " lives. Example: Blocks/Button/ButtonBlock.cshtml, then '~/Blocks/' should be returned");

        var viewPath = string.Format(blockComponentFolderPath + "{0}/{1}.cshtml", blockName, blockName);

        var view = ViewEngine.GetView(null, viewPath, false);

        if (!view.Success)
            viewPath = string.Format(blockComponentFolderPath + "{0}/{1}.cshtml", blockName, "Index");

        return await Task.FromResult(View(viewPath, currentBlock));
    }
}