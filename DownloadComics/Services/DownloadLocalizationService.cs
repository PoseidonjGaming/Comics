using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ComicsLocalizationLib;

namespace DownloadComics.Services
{
    public class DownloadLocalizationService : LocalizationService
    {
        protected override IEnumerable<Dictionary<string, string>> LoadAdditionnalLayers(string lang)
        {
            yield return ReadLang(GetRessource($"{lang}.json", "DownloadComics.Resources.Langs"));
        }
    }
}
