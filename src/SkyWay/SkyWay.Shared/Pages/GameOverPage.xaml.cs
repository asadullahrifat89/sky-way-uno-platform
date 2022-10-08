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
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace SkyWay
{
    public sealed partial class GameOverPage : Page
    {
        #region Fields

        private readonly IBackendService _backendService;

        #endregion

        #region Ctor

        public GameOverPage()
        {
            this.InitializeComponent();
            _backendService = (Application.Current as App).Host.Services.GetRequiredService<IBackendService>();
            this.Loaded += GameOverPage_Loaded;
        }


        #endregion

        #region Events

        #region Page

        private async void GameOverPage_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: set localization
            SetGameResults();
            ShowUserName();

            // if user has not logged in or session has expired
            if (!GameProfileHelper.HasUserLoggedIn() || SessionHelper.HasSessionExpired())
            {
                SetLoginContext();
            }
            else
            {
                this.RunProgressBar();

                if (await SubmitScore())
                {
                    SetLeaderboardContext(); // if score submission was successful make leaderboard button visible
                }
                else
                {
                    SetLoginContext();
                }

                this.StopProgressBar();
            }
        }

        #endregion

        #region Buttons

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(LoginPage));
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(GamePage));
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToPage(typeof(LeaderboardPage));
        }

        #endregion

        #endregion

        #region Methods

        #region Logic

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

        private void SetGameResults()
        {
            ScoreText.Text = LocalizationHelper.GetLocalizedResource("YOUR_SCORE");
            ScoreNumberText.Text = PlayerScoreHelper.PlayerScore.Score.ToString("#");         
            CollectiblesCollectedText.Text = $"{LocalizationHelper.GetLocalizedResource("COLLECTIBLES_COLLECTED")} x " + PlayerScoreHelper.PlayerScore.CollectiblesCollected;
        }

        private void SetLeaderboardContext()
        {
            SignupPromptPanel.Visibility = Visibility.Collapsed;
            LeaderboardButton.Visibility = Visibility.Visible;
        }

        private void SetLoginContext()
        {
            // submit score on user login, or signup then login
            PlayerScoreHelper.GameScoreSubmissionPending = true;

            SignupPromptPanel.Visibility = Visibility.Visible;
            LeaderboardButton.Visibility = Visibility.Collapsed;
        }

        private void ShowUserName()
        {
            if (GameProfileHelper.HasUserLoggedIn())
            {
                UserName.Text = GameProfileHelper.GameProfile.User.UserName;
                UserPicture.Initials = GameProfileHelper.GameProfile.Initials;
                PlayerNameHolder.Visibility = Visibility.Visible;
            }
            else
            {
                PlayerNameHolder.Visibility = Visibility.Collapsed;
            }
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
