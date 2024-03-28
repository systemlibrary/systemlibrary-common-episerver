using System;
using System.Threading.Tasks;

using EPiServer.Shell.Security;
using EPiServer.Web;

using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver;

public partial class WebApplicationInitializer : IBlockingFirstRequestInitializer
{
    UIUserProvider _uIUserProvider;
    UIRoleProvider _uIRoleProvider;
    ISiteDefinitionRepository _siteDefinitionRepository;

    internal static bool HasInitialized = false;

    public WebApplicationInitializer(ISiteDefinitionRepository siteDefinitionRepository)
    {
        try
        {
            _uIUserProvider = Services.Get<UIUserProvider>();
            _uIRoleProvider = Services.Get<UIRoleProvider>();
        }
        catch
        {
            // services.AddCmsAspNetIdentity not invoked, app is running without the AspNetIdentity
        }
        _siteDefinitionRepository = siteDefinitionRepository;
    }

    public bool CanRunInParallel => false;

    public Task InitializeAsync(HttpContext httpContext)
    {
        Dump.Write("Init Async");
        System.IO.File.AppendAllText(@"C:\Logs\textxtt.txt", "hehe\n");
        HasInitialized = true;
        try
        {
            Dump.Write("Init Async");
            if (_uIUserProvider == null)
            {
                Dump.Write("Init Async Ignored");
                return Task.FromResult(0);
            }

            if (IsAnyUserAlreadyRegisteredInDatabase())
            {
                Dump.Write("Init Async not needed");
                return Task.FromResult(0);
            }

            Log.Info("Running first time setup due to 0 users in the DB");

            InitializedSiteDefinitions(httpContext);

            InitializeSystemPropertiesSortIndex();

            InitializeLanguages();

            Dump.Write("Init Async ran");

            return Task.FromResult(CreateAdminUser());
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        Dump.Write("Init Async done");
        return Task.FromResult(0);
    }
}
