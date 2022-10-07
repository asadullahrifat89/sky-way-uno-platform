using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace SkyWay
{
    public static class AssetHelper
    {
        #region Methods

        public static async void PreloadAssets(ProgressBar progressBar)
        {
            progressBar.IsIndeterminate = false;
            progressBar.ShowPaused = false;
            progressBar.Value = 0;
            progressBar.Minimum = 0;
            progressBar.Maximum = Constants.ELEMENT_TEMPLATES.Length + Constants.SOUND_TEMPLATES.Length;

            foreach (var uri in Constants.ELEMENT_TEMPLATES.Select(x => x.Value).ToArray())
            {
                await GetFileAsync(uri, progressBar);
            }

            foreach (var uri in Constants.SOUND_TEMPLATES.Select(x => x.Value).ToArray())
            {
                await GetFileAsync(new Uri($"ms-appx:///{uri}"), progressBar);
            }
        }

        private static async Task GetFileAsync(Uri uri, ProgressBar progressBar)
        {
            await StorageFile.GetFileFromApplicationUriAsync(uri);
            progressBar.Value++;
        }

        #endregion
    }
}
