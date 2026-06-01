namespace ComicsLocalizationLib.Resources
{
    public class LanguageOption(string code, string displayName)
    {
        public string Code { get; set; } = code;
        public string DisplayName { get; set; } = displayName;
        public string FlagPath => $"/Assets/{Code}.png";
    }
}
