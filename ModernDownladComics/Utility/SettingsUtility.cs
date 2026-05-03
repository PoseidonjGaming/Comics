using ComicsLib.Models;
using System;
using Windows.ApplicationModel;
using Windows.Management.Core;
using Windows.Storage;

namespace ModernDownloadComics.Utility
{
    public class SettingsUtility
    {
        private static readonly Lazy<Options> _applicationData = new(new Options());
        

        public static void GetSetting<T>(string name)
        {

            
        }

        public static void SetSetting<T>(string name, T value)
        {
           
        }
    }
}
