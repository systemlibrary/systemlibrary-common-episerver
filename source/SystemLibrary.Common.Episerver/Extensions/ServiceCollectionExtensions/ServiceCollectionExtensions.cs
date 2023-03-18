using System.Collections.Generic;
using System.Linq;

using EPiServer.ServiceLocation;

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

using React.AspNet;

using SystemLibrary.Common.Episerver.Initialize;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Service collection extensions
/// </summary>
public static partial class ServiceCollectionExtensions
{
    internal static CommonEpiserverApplicationServicesOptions Options;

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
    public static IServiceCollection CommonEpiserverApplicationServices(this IServiceCollection services, CommonEpiserverApplicationServicesOptions options = null)
    {
        SetOptions(options);

        services.CommonWebApplicationServices(Options);

        ServiceCollectionFirstTime(services, options);

        ServiceCollectionDisplays(services, options);

        ServiceCollectionCookies(services, options);

        ServiceCollectionCaching(services, options);

        ServiceCollectionCompression(services, options);

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
