using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Policy;
using Windows.Storage;

namespace SkyWay
{
    public static class LocalizationHelper
    {
        #region Methods

        public static async void LoadLocalizationKeys()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/localization.json"));
            var content = await FileIO.ReadTextAsync(file);
            Constants.LOCALIZATION_KEYS = JsonConvert.DeserializeObject<LocalizationKey[]>(content);

#if DEBUG
            Console.WriteLine("Localization Keys Count:" + Constants.LOCALIZATION_KEYS.Length); 
#endif
        }

        public static string GetLocalizedResource(string resourceKey)
        {
            var localizationTemplate = Constants.LOCALIZATION_KEYS.FirstOrDefault(x => x.Key == resourceKey);
            return localizationTemplate?.CultureValues.FirstOrDefault(x => x.Culture == App.CurrentCulture).Value;
        }

        public static void SetLocalizedResource(UIElement uIElement)
        {
            var localizationTemplate = Constants.LOCALIZATION_KEYS.FirstOrDefault(x => x.Key == uIElement.Name);

            if (localizationTemplate is not null)
            {
                var value = localizationTemplate?.CultureValues.FirstOrDefault(x => x.Culture == App.CurrentCulture).Value;

                if (uIElement is TextBlock textBlock)
                    textBlock.Text = value;
                else if (uIElement is TextBox textBox)
                    textBox.Header = value;
                else if (uIElement is PasswordBox passwordBox)
                    passwordBox.Header = value;
                else if (uIElement is Button button)
                    button.Content = value;
                else if (uIElement is ToggleButton toggleButton)
                    toggleButton.Content = value;
                else if (uIElement is HyperlinkButton hyperlinkButton)
                    hyperlinkButton.Content = value;
                else if (uIElement is CheckBox checkBox)
                    checkBox.Content = value;
            }
        }

        #endregion        
    }
}
