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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SkyWay
{
    public sealed partial class LoginPage : Page
    {
        #region Fields

        private readonly IBackendService _backendService;

        #endregion

        #region Ctor

        public LoginPage()
        {
            this.InitializeComponent();
            _backendService = (Application.Current as App).Host.Services.GetRequiredService<IBackendService>();
        }

        #endregion

        #region Events

        #region Buttons

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(StartPage));
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: navigate to sign up page
            //NavigateToPage(typeof(SignUpPage));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginButton.IsEnabled)
                await PerformLogin();
        }

        #endregion

        #region Input Fields

        private void UserNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableLoginButton();
        }

        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableLoginButton();
        }

        private async void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && LoginButton.IsEnabled)
                await PerformLogin();
        }

        #endregion

        #endregion

        #region Methods      

        #region Logic

        private async Task PerformLogin()
        {
            this.RunProgressBar();

            if (await Authenticate() && await GetGameProfile() && await GenerateSession())
            {
                if (PlayerScoreHelper.GameScoreSubmissionPending)
                {
                    if (await SubmitScore())
                        PlayerScoreHelper.GameScoreSubmissionPending = false;
                }

                this.StopProgressBar();

                //TODO: navigate to leaderboard page
                //NavigateToPage(typeof(LeaderboardPage));

                NavigateToPage(typeof(StartPage));
            }           
        }

        private async Task<bool> Authenticate()
        {
            (bool IsSuccess, string Message) = await _backendService.AuthenticateUser(
                userNameOrEmail: UserNameBox.Text.Trim(),
                password: PasswordBox.Text.Trim());

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            return true;
        }

        private async Task<bool> GetGameProfile()
        {
            (bool IsSuccess, string Message, _) = await _backendService.GetUserGameProfile();

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            return true;
        }

        private async Task<bool> GenerateSession()
        {
            (bool IsSuccess, string Message) = await _backendService.GenerateUserSession();

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            return true;
        }

        private async Task<bool> SubmitScore()
        {
            (bool IsSuccess, string Message) = await _backendService.SubmitUserGameScore(PlayerScoreHelper.PlayerScore.Score);

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            return true;
        }

        private void EnableLoginButton()
        {
            LoginButton.IsEnabled = !UserNameBox.Text.IsNullOrBlank() && !PasswordBox.Text.IsNullOrBlank();
        }

        #endregion

        #region Page

        private void NavigateToPage(Type pageType)
        {
            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            App.NavigateToPage(pageType);
        }

        #endregion

        #endregion
    }
}
