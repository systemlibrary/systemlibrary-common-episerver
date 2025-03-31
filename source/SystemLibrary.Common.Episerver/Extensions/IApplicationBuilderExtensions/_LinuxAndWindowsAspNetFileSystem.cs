using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using React.AspNet;

using SystemLibrary.Common.Framework;

namespace SystemLibrary.Common.Episerver.Extensions;

internal class LinuxAndWindowsAspNetFileSystem : AspNetFileSystem
{
    readonly IWebHostEnvironment env;

    static string CurrDir;

    static LinuxAndWindowsAspNetFileSystem()
    {
        CurrDir = Directory.GetCurrentDirectory();
    }

    public LinuxAndWindowsAspNetFileSystem(IWebHostEnvironment env) : base(env)
    {
        this.env = env;
    }

    public override string MapPath(string path)
    {
        if (path == null) return null;
        if (path == "") return "";
        if (path == "~/") return "~/";
        if (path == "/") return "/";

        if (path == null || path == "") return "/";

        if (path.StartsWith("~"))
        {
            path = path.Substring(1);
        }

        // Try checking if file exists 'as is', an absolute path was registered
        if (SafeFileExists(path))
        {
            return path;
        }

        // Convert path to webroot or content rooted path, depending on which file lookup succeeded
        return ReturnMappedWebRootOrConentRoot(path);
    }

    string ReturnMappedWebRootOrConentRoot(string path)
    {
        if (path[0] == '/')
        {
            path = path.Substring(1);
        }

        if (env.WebRootPath != null)
        {
            var webRooted = Path.Combine(env.WebRootPath, path);

            if (SafeFileExists(webRooted))
            {
                return webRooted;
            }
        }

        var contentRooted = Path.Combine(EnvironmentConfig.ContentRootPath, path);

        if (SafeFileExists(contentRooted))
        {
            return contentRooted;
        }

        return path;
    }

    static bool SafeFileExists(string path)
    {
        try
        {
            return File.Exists(path);
        }
        catch
        {
            return false;
        }
    }
}
