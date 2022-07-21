namespace SystemLibrary.Common.Episerver.Extensions;

public class CommonEpiserverApplicationServicesOptions : Web.Extensions.CommonWebApplicationServicesOptions
{
    public bool RegisterDisplays = true;
    public bool CmsUsersSlidingExpiration = true;
    public int CmsUserSessionDurationMinutes = 120;

    public bool ConfigureApplicationCookie = true;

    public string DefaultAdminEmail = "demo@systemlibrary.com";
    public string DefaultAdminPassword = "Demo123!";

    /// <summary>
    /// A comma separated list of languageId's that comes with Episerver
    /// 
    /// Set to null or blank, if you do not want to do anything with languages, so the default that you were used to (15 default languages added, while sv and en is enabled, will then be the result)
    /// 
    /// Example of language id's:
    /// sv, en, da, fr, de, fi, no, nn, nl-BE, nl, en-AU, it-IT
    /// </summary>
    public string InitialLanguagesEnabled = "sv,en";

    public bool HideSuggestedContentTypes = true;
    public bool HidePropertyCategoryList = true;
}