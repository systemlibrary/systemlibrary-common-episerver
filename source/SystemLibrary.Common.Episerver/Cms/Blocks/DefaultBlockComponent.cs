using System;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms.Blocks;

[TemplateDescriptor(Inherited = true)]
public class DefaultBlockComponent : AsyncBlockComponentBase<BlockData>
{
    internal static string DefaultBlockComponentFolderPath;
    protected override async Task<IViewComponentResult> InvokeComponentAsync(BlockData currentBlock)
    {
        if (DefaultBlockComponentFolderPath == null)
            throw new Exception("DefaultBlockComponentFolderPath is null. Please specify it during app.CommonEpiserverApplicationBuilder options, set in your startup/program. It should be the default root folder where block lives in, for instance ~/Content/Blocks/");

        if (!DefaultBlockComponentFolderPath.StartsWith("~") || !DefaultBlockComponentFolderPath.EndsWith("/"))
            throw new Exception("DefaultBlockComponentFolderPath must start with ~/ and end with /");

        var blockName = currentBlock.GetOriginalType().Name;

        var viewPath = string.Format(DefaultBlockComponentFolderPath + "{0}/{1}.cshtml", blockName, blockName);

        var view = ViewEngine.GetView(null, viewPath, false);
        
        if(!view.Success)
            viewPath = string.Format(DefaultBlockComponentFolderPath + "{0}/{1}.cshtml", blockName, "Index");

        return await Task.FromResult(View(viewPath, currentBlock));
    }
}