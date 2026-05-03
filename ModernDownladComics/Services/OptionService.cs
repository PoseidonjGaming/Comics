using ComicsLib.Models;
using ComicsLib.Services;
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

        public OptionService()
        {
            _optionDirectory = Path.Combine(FileService.CurrentDir, "Settings");
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

            FileService.CreateFolder(_optionDirectory);

            return FileService.ReadFile<Options>(_optionPath) ?? new();
        }
        public Options GetOptions()
        {
            return _options.Value;
        }

        public void SaveOptions()
        {
            FileService.WriteFile<Options>(_optionPath, Options);
        }
    }
}
