using System.Reflection;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Episerver.Cms.Properties;

public abstract class BasePropertyController : Controller
{
    static int ClientCacheSeconds = 43200;
    static Assembly _CurrentAssembly;
    protected Assembly CurrentAssembly => _CurrentAssembly != null ? _CurrentAssembly :
        (_CurrentAssembly = Assembly.GetExecutingAssembly());

    protected void AddCacheHeaders()
    {
        if (Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Remove("Cache-Control");

        Response.Headers.Add("Cache-Control", "max-age=" + ClientCacheSeconds);
    }
}
