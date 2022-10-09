using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace SkyWay
{
    public sealed partial class HowToPlayPage : Page
    {
        #region Ctor

        public HowToPlayPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        #region Page

        #endregion

        #region Buttons

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(GamePage));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var itemsCount = InstructionsContainer.Items.Count - 1;

            // once the last instruction is reached, make the start game button visible and hide the next button
            if (InstructionsContainer.SelectedIndex == itemsCount)
            {
                // traverse back to first instruction
                for (int i = 0; i < itemsCount; i++)
                {
                    InstructionsContainer.SelectedIndex--;
                }

                NextButton.Visibility = Visibility.Collapsed;
                PlayButton.Visibility = Visibility.Visible;
            }
            else
            {
                InstructionsContainer.SelectedIndex++;
            }

            SoundHelper.PlaySound(SoundType.MENU_SELECT);
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(StartPage));
        }

        #endregion

        #endregion

        #region Methods

        #region Page

        private void NavigateToPage(Type pageType)
        {
            if (pageType == typeof(GamePage))
                SoundHelper.StopSound(SoundType.INTRO);

            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            App.NavigateToPage(pageType);
        }

        #endregion

        #endregion
    }
}
