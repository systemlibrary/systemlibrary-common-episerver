using System.Collections.Concurrent;

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
using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.App;
using SystemLibrary.Common.Framework.Extensions;

using ContentType = EPiServer.DataAbstraction.ContentType;

namespace SystemLibrary.Common.Episerver;

public enum Hosting
{
    /// <summary>
    /// IIS or IIS Express
    /// </summary>
    IIS,
    /// <summary>
    /// Kestrel
    /// </summary>
    Kestrel,
    /// <summary>
    /// Unknown returns a default app builder only
    /// </summary>
    Unknown = 9999
}

/// <summary>
/// Base Cms functions to use anywhere within your application
/// <para>- No more injecting things that you always need</para>
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
    protected internal static IContentRepository ContentRepository => _ContentRepository ??= Services.Get<IContentRepository>();
    static IContentRepository _ContentRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static IContentVersionRepository ContentVersionRepository => _ContentVersionRepository ??= Services.Get<IContentVersionRepository>();
    static IContentVersionRepository _ContentVersionRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static ILanguageBranchRepository LanguageBranchRepository => _LanguageBranchRepository ??= Services.Get<ILanguageBranchRepository>();
    static ILanguageBranchRepository _LanguageBranchRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static IContentTypeRepository ContentTypeRepository => _ContentTypeRepository ??= Services.Get<IContentTypeRepository>();
    static IContentTypeRepository _ContentTypeRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected internal static ProjectRepository ProjectRepository => _ProjectRepository ??= Services.Get<ProjectRepository>();
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
    /// Returns current block as T based on current request and its context [route data], or null
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
    /// <para>Creates a default CMS host builder</para>
    /// - the 'T' is usually your 'Program.cs' or 'Startup.cs'
    /// </summary>
    /// <param name="hosting">
    /// IIS returns host builder configured for IIS/IIS express
    /// <para>Kestrel returns a host builder configured for Kestrel</para>
    /// <para>Unknown returns a host builder without configuring defaults for the CMS nor hosting</para>
    /// </param>
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
    public static IHostBuilder CreateHostBuilder<T>(string[] args, Hosting hosting, string appSettingsFullPath = null, string[] additionalConfigurationsFullPath = null, bool reloadOnConfigChange = false) where T : class
    {
        if (appSettingsFullPath.IsNot())
            appSettingsFullPath = AppContext.BaseDirectory + "appSettings.json";

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment.IsNot())
            environment = EnvironmentConfig.Current.Name;

        var hostBuilder = Host.CreateDefaultBuilder(args)
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
            });

        if (hosting == Hosting.Kestrel)
        {
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel();
                webBuilder.UseStartup<T>();
            })
             .ConfigureCmsDefaults();
        }
        else if (hosting == Hosting.IIS)
        {
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                if (environment.Is())
                    webBuilder.UseEnvironment(environment);

                webBuilder.UseStartup<T>();
            })
            .ConfigureCmsDefaults();
        }

        return hostBuilder;
    }

    static ConcurrentDictionary<string, string> PrimaryHostUrlCache = new ConcurrentDictionary<string, string>();

    /// <summary>
    /// The Primary Host Url for the current SIte based on Current Request registered inside 'Managed Websites' in Episerver CMS
    /// <para>Gets the first primary host type that is not a wildcard and contains a dot</para>
    /// <para>Fallback to first primary host type that is not a wildcard</para>
    /// <para>Fallback to first undefined host type that is not a wildcard and contains a dot</para>
    /// <para>Fallback to first undefined host type that is not a wildcard</para>
    /// <para>Fallback to Url registered in the Url field of the 'Site'</para>
    /// <para>Fallback to http://localhost in a non web context</para>
    /// </summary>
    /// <remarks>
    /// <para>Returns Primary Host if found</para>
    /// <para>Fallback to first Undefined Type Host, that is not wildcard and contains a dot (.)</para>
    /// <para>Fallback again to first Undefined Type Host, that is not wildcard, for instance "localhost:51011"</para>
    /// <para>Fallback if no host was found, uses the Site Url configred</para>
    /// <para>Last fallback, if no Site Url, for instance in a non web context (unit test/console), fall back to http://localhost</para>
    /// Remember to register the proper site and hosts under 'Manage Websites' in Episerver CMS, which requires app restart after configuration changes to take affect
    /// </remarks>
    public static string PrimaryHostUrl
    {
        get
        {
            var siteDefinition = SiteDefinition.Current;

            return PrimaryHostUrlCache.Cache(siteDefinition.Name, () =>
            {
                var scheme = HttpContextInstance.Current.Request?.Scheme;

                if (scheme.IsNot())
                    scheme = "http";

                var portNumber = "";
                var port = HttpContextInstance.Current.Request?.Host;

                if (port.HasValue && port.Value.Port != 0 && port.Value.Port != 80)
                {
                    portNumber = ":" + port.Value;
                    if (portNumber.Length == 1)
                        portNumber = "";
                }

                string temp;
                if (siteDefinition == null)
                {
                    temp = scheme + "://localhost" + portNumber;
                }
                else
                {
                    var siteUri = siteDefinition.SiteUrl;

                    if (siteUri.Is() && siteUri.Scheme.Is())
                        scheme = siteUri.Scheme;

                    var host = siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Primary && !h.IsWildcardHost() && h.Name.Contains("."));

                    host ??= siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Primary && !h.IsWildcardHost());

                    host ??= siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Undefined && !h.IsWildcardHost() && h.Name.Contains("."));

                    host ??= siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Undefined && !h.IsWildcardHost());

                    if (host != null)
                        siteUri = host.Url;

                    if (siteUri.IsNot())
                        temp = scheme + "://localhost" + portNumber;

                    else
                    {
                        if (siteUri.Scheme.Is())
                            scheme = siteUri.Scheme;

                        if (siteUri.Port != 0 && siteUri.Port != 80 && siteUri.Port != 443)
                            temp = scheme + "://" + siteUri.Host + ":" + siteUri.Port;
                        else
                            temp = scheme + "://" + siteUri.Host;
                    }
                }

                if (temp.EndsWith("/", StringComparison.Ordinal))
                    temp = temp.Substring(0, temp.Length - 1);

                return temp;
            });
        }
    }

    static IEnumerable<ContentType> _AllContentTypes;
    static IEnumerable<ContentType> AllContentTypes
    {
        get
        {
            _AllContentTypes ??= ContentTypeRepository.List();
            return _AllContentTypes;
        }
    }

    /// <summary>
    /// Returns all 'ContentType' that either inherits, implements or that is of type T
    /// </summary>
    public static IEnumerable<ContentType> GetContentTypes<T>() where T : class
    {
        var contentTypes = AllContentTypes;

        var type = typeof(T);

        foreach (var contentType in contentTypes)
        {
            if (contentType.ModelType == type)
                yield return contentType;

            else if (contentType.ModelType.Inherits(type))
                yield return contentType;
        }
    }

    /// <summary>
    /// Returns all 'IContent' that either inherits, implements or that is of type T
    /// <para>- Returns the latest version based on WorkId per ID</para>
    /// <para>- It filters on Deleted, so no deleted items are returned, but if latest is a Draft, that is returned</para>
    /// </summary>
    public static IEnumerable<T> GetAllLatestVersionsOfContentType<T>() where T : class
    {
        var contentTypes = GetContentTypes<T>();

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
                        if (page.IsDeleted) continue;

                        if (!page.IsPublished())
                        {
                            // TODO: Implement a fallback if current work ID is larger than 0, then try get previous workID that was published, if any...
                        }

                        yield return page as T;
                    }
                }
            }
        }
    }
}