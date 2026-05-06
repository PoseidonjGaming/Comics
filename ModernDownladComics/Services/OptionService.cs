using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using System;
using System.IO;

namespace ModernDownladComics.Services
{
    internal class OptionService : ISettingsService
    {
        private readonly Lazy<Options> _options;
        public Options Options => _options.Value;

        public readonly string _optionPath;
        private readonly string _optionDirectory;

        public OptionService(IPathService pathService)
        {
            _optionDirectory = Path.Combine(pathService.GetAppRoot(), "Settings");
            _optionPath = Path.Combine(_optionDirectory,
                $"{Environment.UserName}_settings.json");
            _options = new(LoadOption);
        }
        private Options LoadOption()
        {

            if (!File.Exists(_optionPath))
            {
                return new();
            }

            FileUtility.CreateFolder(_optionDirectory);

            return FileUtility.ReadFile<Options>(_optionPath) ?? new();
        }
        public Options GetOptions()
        {
            return _options.Value;
        }

        public void SaveOptions()
        {
            FileUtility.WriteFile<Options>(_optionPath, Options);
        }
    }
}
