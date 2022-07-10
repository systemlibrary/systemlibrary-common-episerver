using System;
using System.Threading.Tasks;

using EPiServer.Shell.Security;
using EPiServer.Web;

using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver.Initialize
{
    public partial class WebApplicationInitializer : IBlockingFirstRequestInitializer
    {
        UIUserProvider _uIUserProvider;
        UIRoleProvider _uIRoleProvider;
        ISiteDefinitionRepository _siteDefinitionRepository;

        public WebApplicationInitializer(UIUserProvider uIUserProvider, UIRoleProvider uIRoleProvider, ISiteDefinitionRepository siteDefinitionRepository)
        {
            _uIUserProvider = uIUserProvider;
            _uIRoleProvider = uIRoleProvider;
            _siteDefinitionRepository = siteDefinitionRepository;
        }

        public bool CanRunInParallel => false;

        public async Task InitializeAsync(HttpContext httpContext)
        {
            try
            {
                InitializedSiteDefinitions(httpContext);

                if (IsAnyUserOrAdminAlreadyRegistered())
                {
                    return;
                }

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
