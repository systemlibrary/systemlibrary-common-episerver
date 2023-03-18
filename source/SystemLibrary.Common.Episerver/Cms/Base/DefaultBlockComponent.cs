using System;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms;

[TemplateDescriptor(Inherited = true)]
public class DefaultBlockComponent : AsyncBlockComponentBase<BlockData>
{
    public static string DefaultBlockComponentFolderPath;
    protected override async Task<IViewComponentResult> InvokeComponentAsync(BlockData currentBlock)
    {
        if (DefaultBlockComponentFolderPath == null)
            throw new Exception("DefaultBlockComponentFolderPath is null, please set it in startup: DefaultBlockComponent.DefaultBlockComponentFolderPath = 'Some default folder to look for blocks'");

        if(!DefaultBlockComponentFolderPath.StartsWith("~") || !DefaultBlockComponentFolderPath.EndsWith("/"))
            throw new Exception("DefaultBlockComponentFolderPath must start with ~/ and end with /");

        var blockName = currentBlock.GetOriginalType().Name;

        return await Task.FromResult(View(string.Format(DefaultBlockComponentFolderPath + "{0}/{1}.cshtml", blockName, blockName), currentBlock));
    }
}