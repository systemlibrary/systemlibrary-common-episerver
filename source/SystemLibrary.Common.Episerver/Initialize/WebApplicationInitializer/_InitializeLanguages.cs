using EPiServer.DataAbstraction;

using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Episerver;

partial class WebApplicationInitializer
{
    static void InitializeLanguages()
    {
        var initialLanguagesEnabled = Extensions.IServiceCollectionExtensions.Options.InitialLanguagesEnabled;

        if (initialLanguagesEnabled.IsNot()) return;

        Log.Information("Initializing languages: " + initialLanguagesEnabled);

        var initialLanguages = initialLanguagesEnabled.Split(',').Select(x => x.Replace(" ", "")).ToArray();

        var languageBranchRepository = Services.Get<ILanguageBranchRepository>();

        var languageBranches = languageBranchRepository?.ListAll().ToArray();

        if (languageBranches == null || languageBranches.Length == 0) return;

        try
        {
            EnableInitialLanguages(initialLanguages, languageBranchRepository, languageBranches);

            DisableNotInitialLanguages(initialLanguages, languageBranchRepository, languageBranches);

            DeleteNotInitialLanguages(initialLanguages, languageBranchRepository, languageBranches);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    static void EnableInitialLanguages(string[] initialLanguages, ILanguageBranchRepository languageBranchRepository, LanguageBranch[] languageBranches)
    {
        var enableMultipleLanguages = initialLanguages.Length > 1;

        foreach (var language in initialLanguages)
        {
            if (language.IsNot()) continue;

            var enabled = false;
            foreach (var languageBranch in languageBranches)
            {
                if (languageBranch?.LanguageID?.Is() != true) continue;

                if (languageBranch.LanguageID.ToLower() == language.ToLower())
                {
                    var enable = languageBranch.CreateWritableClone();
                    enable.Enabled = true;
                    if (enableMultipleLanguages)
                        enable.URLSegment = languageBranch.LanguageID.Trim().ToLower();

                    languageBranchRepository.Save(enable);
                    enabled = true;
                    break;
                }
            }

            if (!enabled)
            {
                var branch = new LanguageBranch(language);

                if (enableMultipleLanguages)
                    branch.URLSegment = language.ToLower();

                branch.RawIconPath = "~/app_themes/default/images/flags/" + language + ".gif";

                languageBranchRepository.Save(branch);
            }
        }
    }

    static void DeleteNotInitialLanguages(string[] enableLanguages, ILanguageBranchRepository languageBranchRepository, LanguageBranch[] languageBranches)
    {
        foreach (var languageBranch in languageBranches)
        {
            if (languageBranch?.LanguageID?.Is() != true ||
                languageBranch.LanguageID == "en") continue;

            if (!enableLanguages.Contains(languageBranch.LanguageID.Trim()))
            {
                languageBranchRepository.Delete(languageBranch.ID);
            }
        }
    }

    static void DisableNotInitialLanguages(string[] enableLanguages, ILanguageBranchRepository languageBranchRepository, LanguageBranch[] languageBranches)
    {
        foreach (var languageBranch in languageBranches)
        {
            if (languageBranch?.LanguageID?.Is() != true) continue;

            if (!enableLanguages.Contains(languageBranch.LanguageID.Trim()))
            {
                if (languageBranch.Enabled)
                {
                    var disable = languageBranch.CreateWritableClone();
                    disable.Enabled = false;
                    languageBranchRepository.Save(disable);
                }
            }
        }
    }
}
