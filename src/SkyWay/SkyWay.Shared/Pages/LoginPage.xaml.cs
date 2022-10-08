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
            this.Loaded += LoginPage_Loaded;
        }

        #endregion

        #region Events

        #region Page

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: set localization

            // if user was already logged in or came here after sign up
            if (PlayerCredentialsHelper.GetCachedPlayerCredentials() is PlayerCredentials authCredentials 
                && !authCredentials.UserName.IsNullOrBlank()
                && !authCredentials.Password.IsNullOrBlank())
            {
                UserNameBox.Text = authCredentials.UserName;
                PasswordBox.Text = authCredentials.Password;
            }
            else
            {
                UserNameBox.Text = null;
                PasswordBox.Text = null;
            }
        }

        #endregion

        #region Buttons

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(StartPage));
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {            
            NavigateToPage(typeof(SignUpPage));
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
                NavigateToPage(typeof(LeaderboardPage));
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
