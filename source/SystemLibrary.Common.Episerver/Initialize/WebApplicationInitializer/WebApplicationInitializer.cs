using System;
using System.Threading.Tasks;

using EPiServer.Shell.Security;
using EPiServer.Web;

using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Initialize
{
    internal partial class WebApplicationInitializer : IBlockingFirstRequestInitializer
    {
        UIUserProvider _uIUserProvider;
        UIRoleProvider _uIRoleProvider;
        ISiteDefinitionRepository _siteDefinitionRepository;

        public WebApplicationInitializer(ISiteDefinitionRepository siteDefinitionRepository)
        {
            try
            {
                _uIUserProvider = Services.Get<UIUserProvider>();
                _uIRoleProvider = Services.Get<UIRoleProvider>();
            }
            catch
            {
                // services.CmsAspNetIdentity not invoked 
            }
            _siteDefinitionRepository = siteDefinitionRepository;
        }

        public bool CanRunInParallel => false;

        public async Task InitializeAsync(HttpContext httpContext)
        {
            try
            {
                if (_uIUserProvider == null) return;

                if (IsAnyUserAlreadyRegisteredInDatabase())
                {
                    return;
                }

                Log.Info("Running first time setup due to 0 users in the DB");

                InitializedSiteDefinitions(httpContext);

                InitializeSystemPropertiesSortIndex();

                InitializeLanguages();

                await CreateAdminUser().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
