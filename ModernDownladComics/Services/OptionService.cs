using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using System;
using System.IO;

namespace ModernDownladComics.Services
{
    internal class OptionService : ISettingsService
    {
        public Options Options { get; set; }

        public readonly string _optionPath;
        private readonly string _optionDirectory;

        public OptionService(IPathService pathService)
        {
            _optionDirectory = Path.Combine(pathService.GetAppRoot(), "Settings");
            _optionPath = Path.Combine(_optionDirectory,
                $"{Environment.UserName}_settings.json");
            Options = LoadOption();
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
            return Options;
        }

        public void SetOptions(Options options)
        {
            Options = options;
        }

        public void SaveOptions()
        {
            FileUtility.WriteFile(_optionPath, Options);
        }
    }
}
