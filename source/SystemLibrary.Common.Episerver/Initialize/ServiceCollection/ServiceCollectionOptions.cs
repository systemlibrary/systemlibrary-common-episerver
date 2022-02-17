using System;

using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Episerver.Initialize
{
    public class ServiceCollectionOptions : Web.Extensions.ServiceCollectionOptions
    {
        public Type LogType = null;
        public bool RegisterDisplays = true;
        public bool CmsUsersSlidingExpiration = true;
        public int CmsUsersSessionDurationMinutes = 60;
        public bool ConfigureApplicationCookie = true;

        public string DefaultAdminEmail = "admin@admin.com";
        public string DefaultAdminPassword = "Admin123!";

        public bool RemoveSuggestedContentTypes = true;
        public ServiceCollectionOptions()
        {
            this.StringOutputFormatter = new StringOutputFormatter();
        }
    }

    public class StringOutputFormatterCustom : StringOutputFormatter
    {
        public StringOutputFormatterCustom()
        {
            SupportedMediaTypes.Add("application/octet-stream");
        }
    }
}