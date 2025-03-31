using Microsoft.AspNetCore.Hosting;

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
        var tmp = path;
        if (tmp[0] == '/')
        {
            tmp = tmp.Substring(1);
        }

        if (env?.WebRootPath != null)
        {
            var webRooted = Path.Combine(env.WebRootPath, tmp);

            if (SafeFileExists(webRooted))
            {
                return webRooted;
            }
        }

        var contentRooted = Path.Combine(EnvironmentConfig.ContentRootPath, tmp);

        if (SafeFileExists(contentRooted))
        {
            return contentRooted;
        }

        if (SafeFileExists(contentRooted.ToLower()))
            return contentRooted.ToLower();

        Log.Error("[AspnetFileSystem] could not find the SSR script, remember case sensitive paths on Linux, returning content rooted path " + contentRooted);

        return contentRooted;
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
