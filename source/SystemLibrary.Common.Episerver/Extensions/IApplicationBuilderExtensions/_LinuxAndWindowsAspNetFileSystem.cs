using Microsoft.AspNetCore.Hosting;

using React.AspNet;

using SystemLibrary.Common.Framework;

namespace SystemLibrary.Common.Episerver.Extensions;

internal class LinuxAndWindowsAspNetFileSystem : AspNetFileSystem
{
    readonly IWebHostEnvironment _hostingEnv;

    public LinuxAndWindowsAspNetFileSystem(IWebHostEnvironment hostingEnv) : base(hostingEnv)
    {
        _hostingEnv = hostingEnv;
    }

    public override string MapPath(string relativePath)
    {
        if (relativePath.StartsWith(_hostingEnv.WebRootPath))
        {
            return relativePath;
        }

        relativePath = relativePath.TrimStart('~');

        try
        {
            if (File.Exists(relativePath)) return relativePath;
        }
        catch
        {
            // Swallow: exception on no access in case path isnt a file, we continue trying
        }

        relativePath = relativePath.TrimStart('/').TrimStart('\\');

        if (_hostingEnv.WebRootPath == null)
        {
            return Path.GetFullPath(Path.Combine(EnvironmentConfig.ContentRootPath, relativePath));
        }

        return Path.GetFullPath(Path.Combine(_hostingEnv.WebRootPath, relativePath));
    }
}