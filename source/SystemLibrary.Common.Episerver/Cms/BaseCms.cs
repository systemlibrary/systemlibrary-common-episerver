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
/// //'startPage' is null if current page is not start page or null if you are inside a block request, else it returns the startpage
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
    static IContentRepository _ContentRepository;
    protected static IContentRepository ContentRepository =>
        _ContentRepository != null ? _ContentRepository :
        (_ContentRepository = Services.Get<IContentRepository>());

    static ILanguageBranchRepository _LanguageBranchRepository;
    protected static ILanguageBranchRepository LanguageBranchRepository => _LanguageBranchRepository != null ? _LanguageBranchRepository :
        (_LanguageBranchRepository = Services.Get<ILanguageBranchRepository>());

    static IContentTypeRepository _ContentTypeRepository;
    protected static IContentTypeRepository ContentTypeRepository =>
        _ContentTypeRepository != null ? _ContentTypeRepository :
        (_ContentTypeRepository = Services.Get<IContentTypeRepository>());

    static ProjectRepository _ProjectRepository;
    protected static ProjectRepository ProjectRepository =>
        _ProjectRepository != null ? _ProjectRepository :
        (_ProjectRepository = Services.Get<ProjectRepository>());

    protected static IContentModelUsage ContentModelUsage => Services.Get<IContentModelUsage>();

    /// <summary>
    /// Returns content or null if not found
    /// </summary>
    public static T Get<T>(ContentReference contentReference) where T : IContentData
    {
        ContentRepository.TryGet(contentReference, out T content);
        return content;
    }

    /// <summary>
    /// Returns content area items filtered by published, permission and personalized items for the current user
    /// </summary>
    public static IEnumerable<T> GetItems<T>(ContentArea contentArea) where T : IContentData
    {
        if (contentArea.IsNot()) return default;

        var contentReferences = contentArea.FilteredItems.Select(x => x.ContentLink);

        return GetItems<T>(contentReferences);
    }

    /// <summary>
    /// Returns all content references as a list of T
    /// </summary>
    public static IEnumerable<T> GetItems<T>(IEnumerable<ContentReference> contentReferences) where T : IContentData
    {
        if (contentReferences.IsNot()) return new List<T>();

        return ContentRepository.GetItems(contentReferences, new LoaderOptions()).Cast<T>();
    }

    /// <summary>
    /// Returns current page as T based on current request, or null
    /// </summary>
    public static T GetCurrentPage<T>() where T : IContentData
    {
        var pageRouteHelper = Services.Get<IPageRouteHelper>();

        return (T)pageRouteHelper?.Content;
    }

    /// <summary>
    /// Returns current block as T based on current request, or null
    /// </summary>
    public static T GetCurrentBlock<T>() where T : IContentData
    {
        throw new Exception("Not yet implemented");
    }

    public static bool IsInEditMode 
    {
        get
        {
            var mode = Services.Get<IContextModeResolver>();

            return mode.CurrentMode == ContextMode.Edit;
        }
    }

    public static bool IsInPreviewMode
    {
        get
        {
            var mode = Services.Get<IContextModeResolver>();

            return mode.CurrentMode == ContextMode.Preview;
        }
    }

    public static IHostBuilder CreateHostBuilder<T>(string[] args, string appSettingsFullPath = null) where T : class
    {
        if(appSettingsFullPath.IsNot())
            appSettingsFullPath = AppContext.BaseDirectory + "appSettings.json";

        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config => config.AddJsonFile(appSettingsFullPath))
            .ConfigureWebHostDefaults(config =>
            {
                    //NOTE: UseEnvironment() does not change anything that is loaded from SystemLibrary.Common.Net it seems
                    //config.UseEnvironment(environment);
                config.UseStartup<T>();
            })
            .ConfigureCmsDefaults();
    }
}
