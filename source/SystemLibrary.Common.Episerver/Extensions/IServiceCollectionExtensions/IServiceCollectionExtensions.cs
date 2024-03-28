using System.Reflection.Metadata;

using EPiServer.Cms.Shell.UI.Configurations;
using EPiServer.Cms.TinyMce;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Web;

using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using React.AspNet;

using SystemLibrary.Common.Episerver.Initialize;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Service collection extensions
/// </summary>
public static partial class IServiceCollectionExtensions
{
    internal static CommonCmsServicesOptions Options;

    /// <summary>
    /// Registers default web-application settings:
    /// - Controllers, Mvc, RazorPages, Default View Locations, ApplicationCookie, Episervers 'Display Channels', Supported Media Types for your controllers to return json, xtml or csv...
    /// 
    /// Note: 
    /// This does not register an Identity (user), if you want that use the generic method  CommonEpiserverServices&lt;T&gt;()
    /// This creates an admin user if there's no users existing in the DB: demo/Demo123!
    /// Admin user's email and password can be changed by passing the option object
    /// </summary>
    /// <example>
    /// You must also manually call these ones after 'CommonEpiserverServices' as Common.Episerver does not have a dependency on the Cms, TinyMce, nor Find (yet)
    /// <code>
    /// public void ConfigureServices(IServiceCollection services) 
    /// {
    ///     services.CommonEpiserverApplicationServices()
    ///         .AddCms()
    ///         .AddTinyMce()
    ///         .AddFind();
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddCommonCmsServices<TLogWriter>(this IServiceCollection services, CommonCmsServicesOptions options = null)
        where TLogWriter : class, ILogWriter
    {
        SetOptions(options);

        services = services.CommonWebApplicationServices(Options);

        services = services.AddTransient<ILogWriter, TLogWriter>();

        services.AddFirstRequestInitializer(options);

        services.AddApplicationCookie(options);

        services = services.AddResponseCaching();

        services = services.AddCms();

        services.AddTinyMce();

        services.AddDisplays(options);

        services.Configure<UIOptions>(opt =>
        {
            opt.WebSocketEnabled = options.WebSocketEnabled;
            opt.AutoPublishMediaOnUpload = true;
            opt.UIShowGlobalizationUserInterface = false;       // Shows only the main language, not the language in the url (one lang enabled)
            opt.InlineBlocksInContentAreaEnabled = false;
            opt.PreviewTimeout = 60000;
        });

        services.Configure<UploadOptions>(x =>
        {
            x.FileSizeLimit = options.UploadLimitBytes;
        });

        services.Configure<IHttpMaxRequestBodySizeFeature>(x =>
        {
            x.MaxRequestBodySize = options.UploadLimitBytes;
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => false;
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
        });

        services
           .AddJsEngineSwitcher(opt => opt.DefaultEngineName = V8JsEngine.EngineName)
           .AddV8();

        services.AddReact();

        return services;
    }

    /// <summary>
    /// Register default web-application settings:
    /// - Controllers, Mvc, RazorPages, Default View Locations, ApplicationCookie, Episervers 'Display Channels', Supported Media Types for your controllers to return json, xtml or csv...
    /// 
    /// Note: 
    /// This creates an admin user if there's no users existing in the DB: demo/Demo123!
    /// Admin user's email and password can be changed by passing the option object
    /// </summary>
    /// <example>
    /// You must also manually call these ones after 'CommonEpiserverServices' as Common.Episerver does not have a dependency on the Cms, TinyMce, nor Find (yet)
    /// <code>
    /// public void ConfigureServices(IServiceCollection services) 
    /// {
    ///     services.CommonEpiserverApplicationServices()
    ///         .AddCms()
    ///         .AddTinyMce()
    ///         .AddFind();
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddCommonCmsServices<T, TLogWriter>(this IServiceCollection services, CommonCmsServicesOptions options = null)
        where T : IdentityUser, IUIUser, new()
        where TLogWriter : class, ILogWriter
    {
        services.AddCmsAspNetIdentity<T>();

        services = AddCommonCmsServices<TLogWriter>(services, options);

        services.AddScoped<T>();

        return services;
    }

    static void SetOptions(CommonCmsServicesOptions options)
    {
        var fallback = new CommonCmsServicesOptions();

        Options = options ?? fallback;

        if (Options.CmsUsersSignedInDurationMinutes == 0)
            Options.CmsUsersSignedInDurationMinutes = fallback.CmsUsersSignedInDurationMinutes;

        if (Options.DefaultAdminEmail.IsNot())
            Options.DefaultAdminEmail = fallback.DefaultAdminEmail;

        if (Options.DefaultAdminPassword.IsNot())
            Options.DefaultAdminPassword = fallback.DefaultAdminPassword;

        if (Options.ViewLocations != null)
            Options.ViewLocations = ViewLocations.AllViews.Add(Options.ViewLocations);
        else
            Options.ViewLocations = ViewLocations.AllViews;
    }
}
