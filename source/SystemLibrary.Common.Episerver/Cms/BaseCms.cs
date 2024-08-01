using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Base Cms functions to use anywhere within your application
/// - No more injecting things that you always need 
/// - No more fiddling around with where and how do I get the ContentRepository? ServiceLocator? ... do it once, in a Cms class
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// //Use the BaseCms directly without inheriting it yourself:
/// 
/// var startPage = BaseCms.GetCurrentPage&lt;StartPage&gt;();
/// //'startPage' is 'StartPage' is current request is of type 'StartPage', return null else (if you are on an article or inside a block...)
/// 
/// //Extend the BaseCms class with your own Cms functions that are common within your application:
/// public class Cms : BaseCms
/// {
///     public ContentType GetFirstContentType() 
///     {
///         // Use the ContentTypeRepository, or the ContentRepository or the ContentModelUsage static members that you get when inheriting BaseCms
///         return ContentTypeRepository.Load(1);
///     }
/// }
/// </code>
/// </example>
public abstract class BaseCms
{
    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static IContentRepository ContentRepository =>
        _ContentRepository != null ? _ContentRepository :
        (_ContentRepository = Services.Get<IContentRepository>());
    static IContentRepository _ContentRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static ILanguageBranchRepository LanguageBranchRepository => _LanguageBranchRepository != null ? _LanguageBranchRepository :
        (_LanguageBranchRepository = Services.Get<ILanguageBranchRepository>());
    static ILanguageBranchRepository _LanguageBranchRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static IContentTypeRepository ContentTypeRepository =>
        _ContentTypeRepository != null ? _ContentTypeRepository :
        (_ContentTypeRepository = Services.Get<IContentTypeRepository>());
    static IContentTypeRepository _ContentTypeRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static ProjectRepository ProjectRepository =>
        _ProjectRepository != null ? _ProjectRepository :
        (_ProjectRepository = Services.Get<ProjectRepository>());
    static ProjectRepository _ProjectRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static IContentModelUsage ContentModelUsage => Services.Get<IContentModelUsage>();

    /// <summary>
    /// Returns current page as T based on current request, or null
    /// </summary>
    /// <example>
    /// <code>
    /// var currentPageAsStartPage = BaseCms.GetCurrentPage&lt;StartPage&gt;();
    /// //null if current request is not StartPage
    /// </code>
    /// </example>
    public static T GetCurrentPage<T>() where T : IContentData
    {
        var pageRouteHelper = Services.Get<IPageRouteHelper>();

        var content = pageRouteHelper?.Content;

        if (content == null) return default;

        return content is T ? (T)content : default;
    }

    /// <summary>
    /// Returns current block as T based on current request, or null
    /// </summary>
    /// <example>
    /// <code>
    /// var currentBlockAsButtonBlock = BaseCms.GetCurrentBlock&lt;ButtonBlock&gt;();
    /// </code>
    /// </example>
    public static T GetCurrentBlock<T>() where T : IContentData
    {
        throw new Exception("Not yet implemented");
    }

    /// <summary>
    /// Returns true if curent request is inside edit mode, else false
    /// </summary>
    /// <example>
    /// <code>
    /// var isInPreviewMode = BaseCms.IsInPreviewMode;
    /// </code>
    /// </example>
    public static bool IsInEditMode
    {
        get
        {
            var mode = Services.Get<IContextModeResolver>();

            return mode.CurrentMode == ContextMode.Edit;
        }
    }

    /// <summary>
    /// Returns true if curent request is inside preview mode, else false
    /// </summary>
    /// <example>
    /// <code>
    /// var isInPreviewMode = BaseCms.IsInPreviewMode;
    /// </code>
    /// </example>
    public static bool IsInPreviewMode
    {
        get
        {
            var mode = Services.Get<IContextModeResolver>();

            return mode.CurrentMode == ContextMode.Preview;
        }
    }

    /// <summary>
    /// CreateHostBuilder()
    /// Creates a default CMS host builder
    /// - the 'T' is usually your 'Program.cs' or 'Startup.cs'
    /// </summary>
    /// <example>
    /// Program.cs/Startup.cs
    /// <code>
    /// public static void Main(string[] args)
    /// {
    ///     var appSettingsPath = AppContext.BaseDirectory + "Configs\\AppSettings\\appSettings.json";
    ///     try
    ///     {
    ///         Cms.CreateHostBuilder&lt;Program&gt;(args, appSettingsPath)
    ///             .Build()
    ///             .Run();
    ///     }
    ///     catch (Exception ex)
    ///     {
    ///         Log.Error(ex);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IHostBuilder CreateHostBuilder<T>(string[] args, string appSettingsFullPath = null, string[] additionalConfigurationsFullPath = null, bool reloadOnConfigChange = false) where T : class
    {
        if (appSettingsFullPath.IsNot())
            appSettingsFullPath = AppContext.BaseDirectory + "appSettings.json";

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment.IsNot())
            environment = EnvironmentConfig.Current.Name;

        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile(appSettingsFullPath);

                if (environment.Is())
                    config.AddJsonFile(appSettingsFullPath.Replace(".json", "") + environment + ".json", optional: true, reloadOnChange: reloadOnConfigChange);

                if (additionalConfigurationsFullPath.Is())
                {
                    foreach (var additionalConfig in additionalConfigurationsFullPath)
                    {
                        config.AddJsonFile(additionalConfig);
                        if (environment.Is())
                            config.AddJsonFile(additionalConfig.Replace(".json", "") + environment + ".json", optional: true, reloadOnChange: reloadOnConfigChange);
                    }
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (environment.Is())
                    webBuilder.UseEnvironment(environment);

                webBuilder.UseStartup<T>();
            })
            .ConfigureCmsDefaults();
    }

    static string _PrimaryHostUrl;
    /// <summary>
    /// Returns the primary site registered in Episerver, or the SiteUrl defined, or if both are null, falls back to 'localhost', returns never null
    /// 
    /// Note: This requires you to register your primary url inside Web Site Settings, under Admin, within the CMS
    /// </summary>
    public static string PrimaryHostUrl
    {
        get
        {
            if (_PrimaryHostUrl == null)
            {
                var appUrl = AppSettings.Current.SystemLibraryCommonEpiserver.AppUrl;
                if (appUrl.Is())
                {
                    if (appUrl.EndsWith("/", StringComparison.Ordinal))
                        appUrl = appUrl.Substring(0, appUrl.Length - 1);

                    _PrimaryHostUrl = appUrl;

                    return _PrimaryHostUrl;
                }

                var siteDefinition = SiteDefinition.Current;

                var scheme = HttpContextInstance.Current?.Request?.Scheme ?? "http";

                var portNumber = "";
                var port = HttpContextInstance.Current?.Request?.Host;

                if (port.HasValue && port.Value.Port != 0 && port.Value.Port != 80)
                {
                    portNumber = ":" + port.Value;
                }

                if (siteDefinition == null)
                {
                    _PrimaryHostUrl = scheme + "://localhost" + portNumber;
                }
                else
                {
                    var siteUri = siteDefinition.SiteUrl;

                    var host = siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Primary && !h.IsWildcardHost());

                    if (host == null)
                        host = siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Undefined && !h.IsWildcardHost() && h.Name.Contains("."));

                    if (host == null)
                        host = siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Undefined && !h.IsWildcardHost());

                    if (host != null)
                        siteUri = host.Url;

                    if (siteUri.IsNot())
                        _PrimaryHostUrl = scheme + "://localhost" + portNumber;

                    else
                    {
                        if (siteUri.Scheme.Is())
                            scheme = siteUri.Scheme;

                        if (siteUri.Port != 0 && siteUri.Port != 80)
                            _PrimaryHostUrl = scheme + "://" + siteUri.Host + ":" + siteUri.Port;
                        else
                            _PrimaryHostUrl = scheme + "://" + siteUri.Host;
                    }
                }

                if (_PrimaryHostUrl.EndsWith("/", StringComparison.Ordinal))
                    _PrimaryHostUrl = _PrimaryHostUrl.Substring(0, _PrimaryHostUrl.Length - 1);

            }
            return _PrimaryHostUrl;
        }
    }

    static IEnumerable<ContentType> _AllContentTypes;
    static IEnumerable<ContentType> AllContentTypes
    {
        get
        {
            if (_AllContentTypes == null)
            {
                _AllContentTypes = ContentTypeRepository.List();
            }
            return _AllContentTypes;
        }
    }

    /// <summary>
    /// Return all 'ContentType's that either inherits or implements or is the specified class or interface
    /// </summary>
    public static IEnumerable<ContentType> GetContentTypesInheriting<T>() where T : class
    {
        var contentTypes = AllContentTypes;

        var type = typeof(T);

        foreach (var contentType in contentTypes)
        {
            if (contentType.ModelType.Inherits(type))
                yield return contentType;
        }
    }

    /// <summary>
    /// Returns all 'ContentData' that inherits, implements or is of type T
    /// - Returns only the latest version based on WorkId per ID
    /// </summary>
    public static IEnumerable<T> GetLatestVersionOfContentType<T>() where T : class
    {
        var contentTypes = GetContentTypesInheriting<T>();

        foreach (var contentType in contentTypes)
        {
            var usages = ContentModelUsage.ListContentOfContentType(contentType);

            if (usages != null)
            {
                var references = usages
                    .GroupBy(c => c.ContentLink.ID)
                    .Select(link => link.OrderByDescending(c => c.ContentLink.WorkID).First())
                    .Select(x => x.ContentLink);

                var pages = references.To<IContent>();

                if (pages != null)
                {
                    foreach (var page in pages)
                    {
                        yield return page as T;
                    }
                }

            }
        }
    }
}

