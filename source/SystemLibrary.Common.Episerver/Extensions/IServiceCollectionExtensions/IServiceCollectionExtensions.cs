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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using React.AspNet;

using SystemLibrary.Common.Episerver.Initialize;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static partial class IServiceCollectionExtensions
{
    internal static CommonCmsServicesOptions Options;

    public static IServiceCollection AddCommonCmsServices<TLogWriter>(this IServiceCollection services, CommonCmsServicesOptions options = null)
        where TLogWriter : class, ILogWriter
    {
        SetOptions(options);

        services = services.CommonWebApplicationServices(Options);

        services = services.AddTransient<ILogWriter, TLogWriter>();

        services.AddApplicationCookie(Options);

        services = services.AddResponseCaching();

        services = services.AddCms();

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

        services.Configure<CookiePolicyOptions>(opt =>
        {
            opt.CheckConsentNeeded = context => false;
            opt.MinimumSameSitePolicy = SameSiteMode.Lax;
        });

        services
           .AddJsEngineSwitcher(opt => opt.DefaultEngineName = V8JsEngine.EngineName)
           .AddV8();

        services.AddFirstRequestInitializer(options);

        services.AddReact();

        return services;
    }

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
