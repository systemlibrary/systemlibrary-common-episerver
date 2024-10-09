using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Abstract;

[Area(Globals.AreaCms)]
[Authorize(Roles = "WebAdmins, Administrators, WebEditors, CmsAdmins, CmsEditors")]
public abstract class InternalBaseController : Controller
{
    static int ClientCacheSeconds = 7200;

    static Assembly _CurrentAssembly;
    protected Assembly CurrentAssembly => _CurrentAssembly ??= Assembly.GetExecutingAssembly();

    protected bool IsCached(object data)
    {
        if (data != null && !Globals.IsDeveloping) return true;

        return false;
    }

    protected void AddCacheHeaders()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        if (Globals.IsDeveloping)
            ClientCacheSeconds = 1;

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);
    }

    protected StringBuilder GetEmbeddedResource(string resourceFolder, string resourceName)
    {
        try
        {
            if (Globals.IsDeveloping)
            {
                return new StringBuilder(System.IO.File.ReadAllText(Globals.LibraryBasePath + resourceFolder + "\\" + resourceName));
            }

            var sb = new StringBuilder(Net.Assemblies.GetEmbeddedResource(resourceFolder, resourceName, CurrentAssembly));

            if (sb.Length == 0)
                Log.Error("[InternalBaseController] " + this.GetType().Name + " could not find resource " + resourceName + " in folder " + resourceFolder);

            return sb;
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return new StringBuilder("");
        }
    }

    protected FileContentResult GetFileContentResult(StringBuilder content, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(content.ToString());

        return new FileContentResult(bytes, contentType);
    }
}
