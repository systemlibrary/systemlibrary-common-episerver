using EPiServer.ServiceLocation;

using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Episerver.Initialize;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

public static partial class ServiceCollectionExtensions
{
    internal static CommonEpiserverApplicationServicesOptions Options;

    /// <summary>
    /// This method registers default web-application settings:
    /// - Controllers, Mvc, RazorPages, Default View Locations, ApplicationCookie, Episervers 'Display Channels', Supported Media Types for your controllers to return json, xtml or csv...
    /// 
    /// Additional notes: 
    /// This does not register a Cms Identity, use the Generic method CommonEpiserverServices&lt;T&gt;() to register the Identity
    /// This creates an admin user if the user do not exists, by default its admin@example.com/Admin123!
    /// The administrators email and password can be changed by passing in the 'ServiceCollectionOptions' variable
    /// 
    /// You must also manually call these ones after 'CommonEpiserverServices' as Common.Episerver does not have a dependency on the Cms, TinyMce, nor Find (yet)
    /// services.CommonEpiserverServices();
    /// services.AddCms();
    /// services.AddTinyMce();
    /// services.AddFind();
    /// </summary>
    public static IServiceCollection CommonEpiserverApplicationServices(this IServiceCollection services, CommonEpiserverApplicationServicesOptions options = null)
    {
        //Services.Collection = services;

        SetOptions(options);

        services.CommonWebApplicationServices(Options);

        ServiceCollectionDisplays(services, options);

        ServiceCollectionCookies(services, options);

        ServiceCollectionCaching(services, options);

        ServiceCollectionCompression(services, options);

        ServiceCollectionFirstTime(services, options);

        return services;
    }

    /// <summary>
    /// This method registers default web-application settings:
    /// - Controllers, Mvc, RazorPages, Default View Locations, CurrentUser, ApplicationCookie, Episervers 'Display Channels', Supported Media Types for your controllers to return json, xtml or csv...
    /// 
    /// Note: 
    /// This creates an admin user if the user do not exists, by default its admin@example.com/Admin123!
    /// The administrators email and password can be changed by passing in the 'ServiceCollectionOptions' variable
    /// 
    /// You must also manually call these ones after 'CommonEpiserverServices' as Common.Episerver does not have a dependency on the Cms, TinyMce, nor Find (yet)
    /// services.CommonEpiserverServices();
    /// services.AddCms();
    /// services.AddTinyMce();
    /// services.AddFind();
    /// </summary>
    public static IServiceCollection CommonEpiserverApplicationServices<T>(this IServiceCollection services, CommonEpiserverApplicationServicesOptions options = null) where T : CurrentUser, new()
    {
        services.AddCmsAspNetIdentity<T>();

        services.AddScoped<T, T>(); //Can this be ignored due to AspnetIdentity above?

        return CommonEpiserverApplicationServices(services, options);
    }

    static void SetOptions(CommonEpiserverApplicationServicesOptions options)
    {
        var fallback = new CommonEpiserverApplicationServicesOptions();

        Options = options ?? fallback;

        if (Options.CmsUserSessionDurationMinutes == 0)
            Options.CmsUserSessionDurationMinutes = fallback.CmsUserSessionDurationMinutes;

        if (Options.DefaultAdminEmail.IsNot())
            Options.DefaultAdminEmail = fallback.DefaultAdminEmail;

        if (Options.DefaultAdminPassword.IsNot())
            Options.DefaultAdminPassword = fallback.DefaultAdminPassword;

        Options.ViewLocations = ViewLocations.AllViews.Add(Options.ViewLocations);
    }
}
