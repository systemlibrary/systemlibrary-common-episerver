using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Episerver.Initialize
{
    public class ServiceCollectionOptions : Web.Extensions.ServiceCollectionOptions
    {
        public bool RegisterDisplays = true;
        public bool CmsUsersSlidingExpiration = true;
        public int CmsUserSessionDurationMinutes = 120;

        public bool ConfigureApplicationCookie = true;

        public string DefaultAdminEmail = "admin@example.com";
        public string DefaultAdminPassword = "Admin123!";

        /// <summary>
        /// A comma seperated list of languageId's that comes with Episerver
        /// 
        /// Set to null or blank, if you do not want to do anything with languages, so the default that you were used to (15 default languages added, while sv and en is enabled, will then be the result)
        /// 
        /// Example of language id's:
        /// sv, en, da, fr, de, fi, no, nn, nl-BE, nl, en-AU, it-IT
        /// </summary>
        public string InitialLanguagesEnabled = "sv,en";

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