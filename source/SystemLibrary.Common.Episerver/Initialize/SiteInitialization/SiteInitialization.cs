using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Shell.Security;
using EPiServer.Web;

using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver.Initialize
{
    public class SiteInitialization : IBlockingFirstRequestInitializer
    {
        UIUserProvider _uIUserProvider;
        UIRoleProvider _uIRoleProvider;
        ISiteDefinitionRepository _siteDefinitionRepository;

        public SiteInitialization(UIUserProvider uIUserProvider, UIRoleProvider uIRoleProvider, ISiteDefinitionRepository siteDefinitionRepository)
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
                RegisterInitialSiteDefinitions(httpContext);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Dump.Write(ex);
            }
            try
            {
                if (await IsAdminUserAlreadyRegistered())
                {
                    return;
                }

                await CreateAdminUser();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Dump.Write(ex);
                throw;
            }
        }

        async Task CreateAdminUser()
        {
            var email = ServiceCollectionExtensions.Options.DefaultAdminEmail;
            var password = ServiceCollectionExtensions.Options.DefaultAdminPassword;
            var userName = email.Substring(0, email.IndexOf('@'));
            var roles = Roles.CmsRoles;
            var result = await _uIUserProvider.CreateUserAsync(userName, password, email, null, null, true);
            if (result.Status == UIUserCreateStatus.Success)
            {
                foreach (var role in roles)
                {
                    var exists = await _uIRoleProvider.RoleExistsAsync(role);
                    if (!exists)
                        await _uIRoleProvider.CreateRoleAsync(role);
                }

                await _uIRoleProvider.AddUserToRolesAsync(result.User.Username, roles);
            }
        }

        async Task<bool> IsAdminUserAlreadyRegistered()
        {
            var email = ServiceCollectionExtensions.Options.DefaultAdminEmail;
            var userName = email.Substring(0, email.IndexOf('@'));

            var res = await _uIUserProvider.GetUserAsync(userName);

            return res?.Username != null;
        }

        void RegisterInitialSiteDefinitions(HttpContext context)
        {
            if (_siteDefinitionRepository?.List()?.Any() == true)
                return;

            var request = context.Request;

            var host = request.Host;

            var site = new SiteDefinition
            {
                Id = Guid.NewGuid(),
                Name = host.Host,
                SiteUrl = new Uri(request.Scheme + "://" + host.Host + ":" + host.Port)
            };

            if (site.Hosts == null)
                site.Hosts = new List<HostDefinition>();

            site.Hosts.Add(new HostDefinition()
            {
                Name = request.Host.Host + ":" + request.Host.Port,
                Type = HostDefinitionType.Primary
            });

            site.Hosts.Add(new HostDefinition()
            {
                Name = HostDefinition.WildcardHostName,
                Type = HostDefinitionType.Undefined
            });

            site.StartPage = ContentReference.RootPage;

            _siteDefinitionRepository.Save(site);
        }
    }
}
