using System;

using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SystemLibrary.Common.Episerver.Display;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Initialize
{
    public static class ServiceCollectionExtensions
    {
        internal static ServiceCollectionOptions Options;

        /// <summary>
        /// This method registers default web-application settings:
        /// - Controllers, Mvc, RazorPages, Default View Locations, ApplicationCookie, Episervers 'Display Channels', Supported Media Types for your controllers to return json, xtml or csv...
        /// 
        /// Note: This does not register a Cms Identity, use the Generic method CommonEpiserverServices&lt;T&gt;() to register the Identity
        /// 
        /// You must also manually call these ones after 'CommonEpiserverServices' as Common.Episerver does not have a dependency on the Cms, TinyMce, nor Find (yet)
        /// services.CommonEpiserverServices();
        /// services.AddCms();
        /// services.AddTinyMce();
        /// services.AddFind();
        /// </summary>
        public static IServiceCollection CommonEpiserverServices(this IServiceCollection services, ServiceCollectionOptions options = null)
        {
            try
            {
                Services.Collection = services;
                
                SetOptions(options);

                Dump.Write(Options);

                services.CommonServices(Options);

                if (Options.RegisterDisplays)
                {
                    services.Configure<DisplayOptions>(displayOption =>
                    {
                        displayOption.Add("desktop", "Desktop", "col-12", "", "epi-icon__layout--full");
                        displayOption.Add("tablet", "Tablet", "col-8", "", "epi-icon__layout--two-thirds");
                        displayOption.Add("mobile", "Mobile", "col-4", "", "epi-icon__layout--one-third");
                    });
                    services.AddSingleton<DesktopResolution>();
                    services.AddSingleton<TabletResolution>();
                    services.AddSingleton<MobileResolution>();
                }

                if (Options.ConfigureApplicationCookie)
                {
                    services.ConfigureApplicationCookie(o =>
                    {
                        o.LoginPath = "/util/login";
                        o.ExpireTimeSpan = TimeSpan.FromMinutes(Options.CmsUserSessionDurationMinutes);
                        o.SlidingExpiration = Options.CmsUsersSlidingExpiration;
                    });
                }

                services.TryAddSingleton<ServiceAccessor<IContentRouteHelper>>(locator => locator.GetInstance<IContentRouteHelper>);

                services.TryAddEnumerable(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton(typeof(IFirstRequestInitializer), typeof(SiteInitialization)));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Dump.Write(ex);
                throw;
            }

            return services;
        }

        static void SetOptions(ServiceCollectionOptions options)
        {
            var fallback = new ServiceCollectionOptions();

            Options = options ?? fallback;

            if (options == null) return;

            if (Options.ViewLocations == null)
                Options.ViewLocations = new EpiViewLocations();

            if(Options.CmsUserSessionDurationMinutes == 0)
                Options.CmsUserSessionDurationMinutes = fallback.CmsUserSessionDurationMinutes;

            if (options.DefaultAdminEmail.IsNot())
                Options.DefaultAdminEmail = fallback.DefaultAdminEmail;

            if(options.DefaultAdminPassword.IsNot())
                Options.DefaultAdminPassword = fallback.DefaultAdminPassword;    
        }

        /// <summary>
        /// This method registers default web-application settings:
        /// - Controllers, Mvc, RazorPages, Default View Locations, CurrentUser, ApplicationCookie, Episervers 'Display Channels', Supported Media Types for your controllers to return json, xtml or csv...
        /// 
        /// You must also manually call these ones after 'CommonEpiserverServices' as Common.Episerver does not have a dependency on the Cms, TinyMce, nor Find (yet)
        /// services.CommonEpiserverServices();
        /// services.AddCms();
        /// services.AddTinyMce();
        /// services.AddFind();
        /// </summary>
        public static IServiceCollection CommonEpiserverServices<T>(this IServiceCollection services, ServiceCollectionOptions options = null) where T : CurrentUser, new()
        {
            services.AddCmsAspNetIdentity<T>();

            services.AddScoped<T, T>(); //Can this be ignored due to AspnetIdentity above?

            return CommonEpiserverServices(services, options);
        }
    }
}