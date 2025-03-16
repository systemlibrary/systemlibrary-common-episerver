using EPiServer.Shell.Security;
using EPiServer.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Internal class only, but marked as Public due to test methods
/// </summary>
public partial class WebApplicationInitializer : IBlockingFirstRequestInitializer
{
    UIUserProvider uIUserProvider;
    UIRoleProvider uIRoleProvider;
    ISiteDefinitionRepository siteDefinitionRepository;

    IWebHostEnvironment env;

    public WebApplicationInitializer(ISiteDefinitionRepository siteDefinitionRepository, IWebHostEnvironment env)
    {
        try
        {
            uIUserProvider = Services.Get<UIUserProvider>();
            uIRoleProvider = Services.Get<UIRoleProvider>();
        }
        catch
        {
            // services.AddCmsAspNetIdentity not invoked, app is running without the AspNetIdentity
        }
        this.siteDefinitionRepository = siteDefinitionRepository;
        this.env = env;
    }

    public bool CanRunInParallel => false;

    public Task InitializeAsync(HttpContext httpContext)
    {
        try
        {
            env.WebRootPath ??= EnvironmentConfig.ContentRootPath;

            if (Extensions.IServiceCollectionExtensions.Options.SkipInitialization)
                return Task.FromResult(0);

            if (uIUserProvider == null)
            {
                return Task.FromResult(0);
            }

            if (IsAnyUserAlreadyRegisteredInDatabase())
            {
                return Task.FromResult(0);
            }

            Log.Information("Running first time setup due to 0 users in the DB");

            InitializedSiteDefinitions(httpContext);

            InitializeSystemPropertiesSortIndex();

            InitializeLanguages();

            return Task.FromResult(CreateAdminUser());
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        return Task.FromResult(0);
    }
}
