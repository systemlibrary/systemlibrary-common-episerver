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

namespace SystemLibrary.Common.Episerver
{
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
    /// //'startPage' is null if current page is not start page, else it returns the startpage
    /// 
    /// //Extend the BaseCms class with your own Cms functions that are common within your application:
    /// public class Cms : BaseCms
    /// {
    ///     public ContentType GetFirstContentType() 
    ///     {
    ///         // Use the ContentTypeRepository, or the ContentRepository or the ContentModelUsage static members
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

        static IContentTypeRepository _ContentTypeRepository;
        protected static IContentTypeRepository ContentTypeRepository =>
            _ContentTypeRepository != null ? _ContentTypeRepository :
            (_ContentTypeRepository = Services.Get<IContentTypeRepository>());

        protected static IContentModelUsage ContentModelUsage => Services.Get<IContentModelUsage>();

        /// <summary>
        /// Returns content or null if not found
        /// </summary>
        public static T Get<T>(ContentReference contentReference) where T : ContentData
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
        public static T GetCurrentPage<T>() where T : ContentData
        {
            var pageRouteHelper = Services.Get<IPageRouteHelper>();

            return pageRouteHelper?.Content as T;
        }

        /// <summary>
        /// Returns current block as T based on current request, or null
        /// </summary>
        public static T GetCurrentBlock<T>() where T : ContentData
        {
            throw new System.Exception("Not yet implemented");
        }

        public static bool IsInEditMode()
        {
            var mode = Services.Get<IContextModeResolver>();

            return mode.CurrentMode == ContextMode.Edit;
        }

        public static bool IsInPreviewMode()
        {
            var mode = Services.Get<IContextModeResolver>();

            return mode.CurrentMode == ContextMode.Preview;
        }

        public static IHostBuilder CreateCmsHostBuilder<T>(string[] args, string environment = "Dev", string appSettingsPath = null) where T : class
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => config.AddJsonFile(appSettingsPath))
                .ConfigureWebHostDefaults(
                config =>
                {
                    config.UseEnvironment(environment);
                    config.UseStartup<T>();
                })
                .ConfigureCmsDefaults();
        }
    }
}
