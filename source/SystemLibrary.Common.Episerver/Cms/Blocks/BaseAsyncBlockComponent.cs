using System;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms.Blocks;

public abstract class BaseAsyncBlockComponent<T> : AsyncBlockComponent<T> where T : BlockData
{
    protected async Task<IViewComponentResult> ResultAsync(string viewName, object model)
    {
        if (!viewName.StartsWith("~"))
            throw new Exception("ComponentResultAsync() requires a full relative path to the view, starting with ~/");

        if (!viewName.EndsWith(".cshtml"))
            viewName = viewName + ".cshtml";

        return await Task.FromResult(View(viewName, model));
    }
}
