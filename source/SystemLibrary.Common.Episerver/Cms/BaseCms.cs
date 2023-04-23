using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using SystemLibrary.Common.Episerver.Extensions;
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
    protected static IContentRepository ContentRepository =>
        _ContentRepository != null ? _ContentRepository :
        (_ContentRepository = Services.Get<IContentRepository>());
    static IContentRepository _ContentRepository;


    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected static ILanguageBranchRepository LanguageBranchRepository => _LanguageBranchRepository != null ? _LanguageBranchRepository :
        (_LanguageBranchRepository = Services.Get<ILanguageBranchRepository>());
    static ILanguageBranchRepository _LanguageBranchRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected static IContentTypeRepository ContentTypeRepository =>
        _ContentTypeRepository != null ? _ContentTypeRepository :
        (_ContentTypeRepository = Services.Get<IContentTypeRepository>());
    static IContentTypeRepository _ContentTypeRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected static ProjectRepository ProjectRepository =>
        _ProjectRepository != null ? _ProjectRepository :
        (_ProjectRepository = Services.Get<ProjectRepository>());
    static ProjectRepository _ProjectRepository;

    /// <summary>
    /// Protected member you can reuse if you inherit BaseCms
    /// </summary>
    protected static IContentModelUsage ContentModelUsage => Services.Get<IContentModelUsage>();

    /// <summary>
    /// Returns content or null if not found
    /// </summary>
    /// <example>
    /// <code>
    /// var page = BaseCms.Get&lt;StartPage&gt;(ContentReference.StartPage);
    /// //page should now be 'StartPage', as ContentReference.StartPage is a PageReference, which inherits 'ContentReference'
    /// //and in most solutions a 'StartPage' always exists
    /// </code>
    /// </example>
    public static T Get<T>(ContentReference contentReference) where T : IContentData
    {
        ContentRepository.TryGet(contentReference, out T content);
        return content;
    }

    /// <summary>
    /// Returns content area items filtered by published, permission and personalized items for the current user
    /// </summary>
    /// <example>
    /// <code>
    /// var pages = BaseCms.GetItems&lt;PageData&gt;(startPage.ContentArea);
    /// </code>
    /// </example>
    public static IEnumerable<T> GetItems<T>(ContentArea contentArea) where T : IContentData
    {
        if (contentArea.IsNot()) return default;

        var contentReferences = contentArea.FilteredItems.Select(x => x.ContentLink);

        return GetItems<T>(contentReferences);
    }

    /// <summary>
    /// Returns all content references as a list of T
    /// </summary>
    /// <example>
    /// <code>
    /// var contentReferences = new List&lt;ContentReference&gt;();
    /// contentReferences.Add(new ContentReference(5));
    /// 
    /// var pages = BaseCms.GetItems&lt;PageData&gt;(contentReferences);
    /// //pages[0] should be start page in most solutions, as thats the ID 5 in the DB
    /// </code>
    /// </example>
    public static IEnumerable<T> GetItems<T>(IEnumerable<ContentReference> contentReferences) where T : IContentData
    {
        if (contentReferences.IsNot()) return new List<T>();

        return ContentRepository.GetItems(contentReferences, new LoaderOptions()).Cast<T>();
    }

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

        return (T)pageRouteHelper?.Content;
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
    ///         Dump.Write(ex);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IHostBuilder CreateHostBuilder<T>(string[] args, string appSettingsFullPath = null) where T : class
    {
        if(appSettingsFullPath.IsNot())
            appSettingsFullPath = AppContext.BaseDirectory + "appSettings.json";

        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config => config.AddJsonFile(appSettingsFullPath))
            .ConfigureCmsDefaults()
            .ConfigureWebHostDefaults(config =>
            {
                //NOTE: UseEnvironment() does not change anything that is loaded from SystemLibrary.Common.Net it seems
                //config.UseEnvironment(environment);
                config.UseStartup<T>();
            });
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
                var appUrl = AppSettings.Current.AppUrl;
                if(appUrl.Is())
                {
                    _PrimaryHostUrl = appUrl;

                    return _PrimaryHostUrl;
                }

               var siteDefinition = SiteDefinition.Current;

                var schema = HttpContextInstance.Current?.Request?.Scheme ?? "http";

                var portNumber = "";
                var port = HttpContextInstance.Current?.Request?.Host;

                if(port.HasValue && port.Value.Port != 0 && port.Value.Port != 80)
                {
                    portNumber = ":" + port.Value;
                }

                if (siteDefinition == null)
                {
                    return schema + "://localhost" + portNumber;
                }
                else
                {
                    var host = siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Primary && !h.IsWildcardHost());
                    
                    if (host == null)
                        host = siteDefinition.Hosts?.FirstOrDefault(h => h.Type == HostDefinitionType.Undefined && !h.IsWildcardHost() && h.Name.Contains("."));

                    if (host == null)
                    {
                        var siteUrl = siteDefinition.SiteUrl;

                        if (siteUrl.IsNot())
                            return schema + "://localhost" + portNumber;

                        else if (siteUrl.Port != 0 && siteUrl.Port != 80)
                            _PrimaryHostUrl = schema + "://" + siteUrl.Host + ":" + siteUrl.Port;
                        else
                            _PrimaryHostUrl = schema + "://" + siteUrl.Host;
                    }
                    else
                    {
                        if (host.Url.Port != 0 && host.Url.Port != 80)
                        {
                            _PrimaryHostUrl = schema + "://" + host.Url.Host + ":" + host.Url.Port;
                        }
                        else
                        {
                            _PrimaryHostUrl = schema + "://" + host.Url.Host + portNumber;
                        }
                    }
                }
            }
            return _PrimaryHostUrl;
        }
    }
}