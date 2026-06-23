using ComicsLib.Models;
using DownloadComics.Models;

namespace DownloadComics.Utilities
{
    public class SettingsUtility
    {
        public static SettingsInputModel ToInputModel(Options options)
        {
            return new SettingsInputModel(options);
        }

        public static Options ToOptions(SettingsInputModel inputModel)
        {
            return new Options
            {
                Comic = inputModel.Comic,
                Hosts = [.. inputModel.Hosts],
                Confirms = [.. inputModel.Confirms],
                ExcludedHosts = [.. inputModel.ExcludedHosts],
                Paths= [.. inputModel.Paths],
                Path = inputModel.Path,
                Lang = inputModel.Lang
            };
        }
    }
}
