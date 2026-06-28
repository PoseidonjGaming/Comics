using ComicsLocalizationLib.Resources;

namespace ComicsLocalizationLib
{
    public interface ILocalizationService
    {
        string Get(string key);
        IEnumerable<LanguageOption> GetLanguage();
        void LoadLang(string lang);
    }
}
