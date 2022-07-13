﻿using System;
using System.Threading.Tasks;

using EPiServer.Shell.Security;
using EPiServer.Web;

using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver.Initialize
{
    internal partial class WebApplicationInitializer : IBlockingFirstRequestInitializer
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
                if (IsAnyUserOrAdminAlreadyRegistered())
                {
                    return;
                }

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
