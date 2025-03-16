using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Configurations;
using EPiServer.Cms.TinyMce;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;

using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using React;
using React.AspNet;

using SystemLibrary.Common.Episerver.Initialize;
using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static partial class IServiceCollectionExtensions
{
    internal static CmsServicesCollectionOptions Options;

    public static IServiceCollection AddCommonCmsServices<T, TLogWriter>(this IServiceCollection services, CmsServicesCollectionOptions options = null)
        where T : IdentityUser, IUIUser, new()
        where TLogWriter : class, ILogWriter
    {
        SetOptions(options);

        // services.AddCms();

        services.AddCmsHost()
            .AddCmsHtmlHelpers()
            .AddCmsTagHelpers()
            .AddCmsUI()
            .AddAdmin()
            .AddVisitorGroupsUI()
            .AddTinyMce()
            .AddCmsAspNetIdentity<T>();

        services.AddFrameworkServices<TLogWriter>(Options);

        services.AddApplicationCookie(Options);

        services.AddTinyMce();

        services.AddDisplays(Options);

        services.Configure<UIOptions>(opt =>
        {
            opt.WebSocketEnabled = Options.WebSocketEnabled;
            opt.AutoPublishMediaOnUpload = Options.AutoPublishMediaOnUpload;
            opt.PreviewTimeout = 60000;
        });

        services.Configure<UploadOptions>(x =>
        {
            x.FileSizeLimit = Options.UploadLimitBytes;
        });

        services.Configure<IHttpMaxRequestBodySizeFeature>(x =>
        {
            x.MaxRequestBodySize = Options.UploadLimitBytes;
        });

        services.Configure<FormOptions>(opt =>
        {
            opt.MultipartBodyLengthLimit = Options.UploadLimitBytes;
        });

        if (Options.AddReactServerSideServices)
            services
                .AddJsEngineSwitcher(opt => opt.DefaultEngineName = V8JsEngine.EngineName)
                .AddV8();

        services.AddFirstRequestInitializer(options);

        if (Options.AddReactServerSideServices)
        {
            services.AddReact();
            ReactSiteConfiguration.Configuration.JsonSerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            ReactSiteConfiguration.Configuration.JsonSerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            ReactSiteConfiguration.Configuration.JsonSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        // services.AddScoped<ContentAreaRenderer, CustomContentAreaRenderer>();

        services.AddScoped<T>();

        return services;
    }

    static void SetOptions(CmsServicesCollectionOptions options)
    {
        var fallback = new CmsServicesCollectionOptions();

        Options = options ?? fallback;

        if (Options.ApplicationCookieDuration == 0)
            Options.ApplicationCookieDuration = fallback.ApplicationCookieDuration;

        if (Options.CookieDuration == 0)
            Options.CookieDuration = fallback.CookieDuration;

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
