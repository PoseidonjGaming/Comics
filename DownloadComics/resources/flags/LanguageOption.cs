using System;
using System.Collections.Generic;
using System.Text;

namespace DownloadComics.resources.flags
{
    public class LanguageOption(string code, string displayName)
    {
        public string Code { get; set; } = code;
        public string DisplayName { get; set; } = displayName;
        public string FlagPath => $"/resources/flags/{Code}.png";
    }
}
