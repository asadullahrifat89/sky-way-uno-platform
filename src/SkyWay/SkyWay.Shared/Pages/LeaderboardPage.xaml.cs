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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Uno.Extensions;

namespace SkyWay
{
    public sealed partial class LeaderboardPage : Page
    {
        #region Fields

        private readonly IBackendService _backendService;

        #endregion

        #region Properties

        public ObservableCollection<GameProfile> GameProfilesCollection { get; set; } = new ObservableCollection<GameProfile>();

        public ObservableCollection<GameScore> GameScoresCollection { get; set; } = new ObservableCollection<GameScore>();

        #endregion

        #region Ctor

        public LeaderboardPage()
        {
            this.InitializeComponent();
            _backendService = (Application.Current as App).Host.Services.GetRequiredService<IBackendService>();

            GameProfilesList.ItemsSource = GameProfilesCollection;
            GameScoresList.ItemsSource = GameScoresCollection;

            this.Loaded += LeaderboardPage_Loaded;
        }

        #endregion

        #region Events

        #region Page

        private async void LeaderboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: set localization            

            this.RunProgressBar();

            if (await GetGameProfile())
                ShowUserName();

            DailyScoreboardToggle.IsChecked = true;

            this.StopProgressBar();
        }

        #endregion

        #region Buttons

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(GamePage));
        }

        private async void AllTimeScoreboardToggle_Click(object sender, RoutedEventArgs e)
        {
            this.RunProgressBar();

            DailyScoreboardToggle.IsChecked = false;
            await GetGameProfiles();

            this.StopProgressBar();
        }

        private async void DailyScoreboardToggle_Click(object sender, RoutedEventArgs e)
        {
            this.RunProgressBar();

            AllTimeScoreboardToggle.IsChecked = false;
            await GetGameScores();

            this.StopProgressBar();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(StartPage));
        }

        #endregion

        #endregion

        #region Methods

        #region Logic

        private async Task<bool> GetGameProfile()
        {
            (bool IsSuccess, string Message, _) = await _backendService.GetUserGameProfile();

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            SetGameScores(
                personalBestScore: GameProfileHelper.GameProfile.PersonalBestScore,
                lastGameScore: GameProfileHelper.GameProfile.LastGameScore);

            return true;
        }

        private async Task<bool> GetGameProfiles()
        {
            GameProfilesCollection.Clear();
            SetListViewMessage(LocalizationHelper.GetLocalizedResource("LOADING_DATA"));

            (bool IsSuccess, string Message, GameProfile[] GameProfiles) = await _backendService.GetUserGameProfiles(pageIndex: 0, pageSize: 10);

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            if (GameProfiles is not null && GameProfiles.Length > 0)
            {
                SetListViewMessage();
                GameProfilesCollection.AddRange(GameProfiles);
                SetLeaderboardPlacements(GameProfilesCollection);
                IndicateCurrentPlayer(GameProfilesCollection.Cast<LeaderboardPlacement>().ToObservableCollection());
            }
            else
            {
                SetListViewMessage(LocalizationHelper.GetLocalizedResource("NO_DATA_AVAILABLE"));
            }

            return true;
        }

        private async Task<bool> GetGameScores()
        {
            GameScoresCollection.Clear();
            SetListViewMessage(LocalizationHelper.GetLocalizedResource("LOADING_DATA"));

            (bool IsSuccess, string Message, GameScore[] GameScores) = await _backendService.GetUserGameScores(pageIndex: 0, pageSize: 10);

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            if (GameScores is not null && GameScores.Length > 0)
            {
                SetListViewMessage();
                GameScoresCollection.AddRange(GameScores);
                SetLeaderboardPlacements(GameScoresCollection);
                IndicateCurrentPlayer(GameScoresCollection.Cast<LeaderboardPlacement>().ToObservableCollection());
            }
            else
            {
                SetListViewMessage(LocalizationHelper.GetLocalizedResource("NO_DATA_AVAILABLE"));
            }

            return true;
        }
       
        private void SetLeaderboardPlacements(dynamic leaderboardPlacements)
        {
            if (leaderboardPlacements.Count > 0)
            {
                // king of the ring
                if (leaderboardPlacements[0] is LeaderboardPlacement firstPlacement)
                {
                    firstPlacement.MedalEmoji = "🥇";
                    firstPlacement.Emoji = "🏆";
                }

                if (leaderboardPlacements.Count > 1)
                {
                    if (leaderboardPlacements[1] is LeaderboardPlacement secondPlacement)
                    {
                        secondPlacement.MedalEmoji = "🥈";
                    }
                }

                if (leaderboardPlacements.Count > 2)
                {
                    if (leaderboardPlacements[2] is LeaderboardPlacement thirdPlacement)
                    {
                        thirdPlacement.MedalEmoji = "🥉";
                    }
                }
            }
        }

        private void IndicateCurrentPlayer(ObservableCollection<LeaderboardPlacement> leaderboardPlacements)
        {
            if (leaderboardPlacements is not null)
            {
                if (leaderboardPlacements.FirstOrDefault(x => x.User.UserName == GameProfileHelper.GameProfile.User.UserName || x.User.UserEmail == GameProfileHelper.GameProfile.User.UserEmail) is LeaderboardPlacement placement)
                {
                    placement.Emoji = "👨‍🚀";
                }
            }
        }

        private void SetListViewMessage(string message = null)
        {
            ListViewMessage.Text = message;
            ListViewMessage.Visibility = message.IsNullOrBlank() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetGameScores(double personalBestScore, double lastGameScore)
        {
            PersonalBestScoreText.Text = LocalizationHelper.GetLocalizedResource("PERSONAL_BEST_SCORE") + ": " + personalBestScore;
            ScoreText.Text = LocalizationHelper.GetLocalizedResource("LAST_GAME_SCORE") + ": " + lastGameScore;
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
