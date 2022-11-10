using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms.Abstract;

public abstract class BaseController : Controller
{
    static int ClientCacheSeconds = 1;

    static Assembly _CurrentAssembly;
    protected Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
        (_CurrentAssembly = Assembly.GetExecutingAssembly());

    protected void AddCacheHeaders()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);
    }

    protected StringBuilder GetEmbeddedResource(string resourceFolder, string resourceName)
    {
        var sb = new StringBuilder(Net.Assemblies.GetEmbeddedResource(resourceFolder, resourceName, CurrentAssembly));

        if(sb.Length == 0)
            Log.Error(this.GetType().Name + " could not find resource " + resourceName + " in folder " + resourceFolder);

        return sb;
    }

    protected FileContentResult GetFileContentResult(StringBuilder content, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(content.ToString());

        return new FileContentResult(bytes, contentType);
    }
}
