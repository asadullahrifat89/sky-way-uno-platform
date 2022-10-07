using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SkyWay
{   
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is string tag)
            {
                LocalizationHelper.CurrentCulture = tag;
                LocalizationHelper.SaveLocalizationCache(tag);
            }
        }
    }
}
