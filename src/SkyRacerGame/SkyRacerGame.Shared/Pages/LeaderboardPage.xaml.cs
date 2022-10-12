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
using System.Threading;

namespace SkyRacerGame
{
    public sealed partial class LeaderboardPage : Page
    {
        #region Fields

        private PeriodicTimer _gameViewTimer;
        private readonly TimeSpan _frameTime = TimeSpan.FromMilliseconds(Constants.DEFAULT_FRAME_TIME);

        private readonly Random _random = new();

        private double _windowHeight, _windowWidth;
        private double _scale;

        private int _gameSpeed = 8;

        private int _markNum;

        private Uri[] _cars;
        private Uri[] _clouds;

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

            _windowHeight = Window.Current.Bounds.Height;
            _windowWidth = Window.Current.Bounds.Width;

            LoadGameElements();
            PopulateGameViews();

            this.Loaded += LeaderboardPage_Loaded;
            this.Unloaded += LeaderboardPage_Unloaded;
        }

        #endregion

        #region Events

        #region Page

        private async void LeaderboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetLocalization();

            this.RunProgressBar();

            if (await GetGameProfile())
                ShowUserName();

            DailyScoreboardToggle.IsChecked = true;

            this.StopProgressBar();

            SizeChanged += GamePage_SizeChanged;
            StartAnimation();
        }

        private void LeaderboardPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= GamePage_SizeChanged;
            StopAnimation();
        }

        private void GamePage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            _windowWidth = args.NewSize.Width;
            _windowHeight = args.NewSize.Height;

            SetViewSize();

#if DEBUG
            Console.WriteLine($"WINDOWS SIZE: {_windowWidth}x{_windowHeight}");
#endif
        }

        #endregion

        #region Buttons

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(GamePage));
        }

        private async void AllTimeScoreboardToggle_Click(object sender, RoutedEventArgs e)
        {
            SoundHelper.PlaySound(SoundType.MENU_SELECT);

            this.RunProgressBar();

            DailyScoreboardToggle.IsChecked = false;
            await GetGameProfiles();

            this.StopProgressBar();
        }

        private async void DailyScoreboardToggle_Click(object sender, RoutedEventArgs e)
        {
            SoundHelper.PlaySound(SoundType.MENU_SELECT);

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
            PersonalBestScoreText.Text = LocalizationHelper.GetLocalizedResource("PersonalBestScoreText") + ": " + personalBestScore;
            LastGameScoreText.Text = LocalizationHelper.GetLocalizedResource("LastGameScoreText") + ": " + lastGameScore;
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

        private void SetViewSize()
        {
            _scale = ScalingHelper.GetGameObjectScale(_windowWidth);

            UnderView.Width = _windowWidth;
            UnderView.Height = _windowHeight;

            OverView.Width = _windowWidth;
            OverView.Height = _windowHeight;
        }

        private void NavigateToPage(Type pageType)
        {
            if (pageType == typeof(GamePage))
                SoundHelper.StopSound(SoundType.INTRO);

            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            App.NavigateToPage(pageType);
        }

        #endregion

        #region Animation

        #region Game

        private void PopulateGameViews()
        {
#if DEBUG
            Console.WriteLine("INITIALIZING GAME");
#endif
            SetViewSize();
            PopulateUnderView();
            PopulateOverView();
        }

        private void LoadGameElements()
        {
            _cars = Constants.ELEMENT_TEMPLATES.Where(x => x.Key == ElementType.CAR).Select(x => x.Value).ToArray();
            _clouds = Constants.ELEMENT_TEMPLATES.Where(x => x.Key == ElementType.CLOUD).Select(x => x.Value).ToArray();
        }

        private void PopulateUnderView()
        {
            // add some cars underneath
            for (int i = 0; i < 10; i++)
            {
                var car = new Car()
                {
                    Width = Constants.CAR_WIDTH * _scale,
                    Height = Constants.CAR_HEIGHT * _scale,
                    IsCollidable = false,
                };

                RandomizeCarPosition(car);
                UnderView.Children.Add(car);
            }

            // add some clouds underneath
            for (int i = 0; i < 15; i++)
            {
                var scaleFactor = _random.Next(1, 4);
                var scaleReverseFactor = _random.Next(-1, 2);

                var cloud = new Cloud()
                {
                    Width = Constants.CLOUD_WIDTH * _scale,
                    Height = Constants.CLOUD_HEIGHT * _scale,
                    RenderTransform = new CompositeTransform()
                    {
                        ScaleX = scaleFactor * scaleReverseFactor,
                        ScaleY = scaleFactor,
                    }
                };

                RandomizeCloudPosition(cloud);
                UnderView.Children.Add(cloud);
            }
        }

        private void PopulateOverView()
        {
            for (int i = 0; i < 1; i++)
            {
                var car = new Player()
                {
                    Width = Constants.PLAYER_WIDTH * _scale,
                    Height = Constants.PLAYER_HEIGHT * _scale,
                };

                RandomizeCarPosition(car);
                OverView.Children.Add(car);
            }
        }

        private void StartAnimation()
        {
#if DEBUG
            Console.WriteLine("GAME STARTED");
#endif            
            RecycleGameObjects();
            RunGame();
        }

        private void RecycleGameObjects()
        {
            foreach (GameObject x in UnderView.Children.OfType<GameObject>())
            {
                switch ((ElementType)x.Tag)
                {
                    case ElementType.CLOUD:
                        {
                            RecyleCloud(x);
                        }
                        break;
                    case ElementType.CAR:
                        {
                            RecyleCar(x);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (GameObject x in OverView.Children.OfType<GameObject>())
            {
                switch ((ElementType)x.Tag)
                {
                    case ElementType.PLAYER:
                        {
                            RecylePlayer(x);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private async void RunGame()
        {
            _gameViewTimer = new PeriodicTimer(_frameTime);

            while (await _gameViewTimer.WaitForNextTickAsync())
            {
                GameViewLoop();
            }
        }

        private void GameViewLoop()
        {
            UpdateGameObjects();
        }

        private void UpdateGameObjects()
        {
            foreach (GameObject x in UnderView.Children.OfType<GameObject>())
            {
                switch ((ElementType)x.Tag)
                {
                    case ElementType.CAR:
                        {
                            UpdateCar(x);
                        }
                        break;
                    case ElementType.CLOUD:
                        {
                            UpdateCloud(x);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (GameObject x in OverView.Children.OfType<GameObject>())
            {
                switch ((ElementType)x.Tag)
                {
                    case ElementType.PLAYER:
                        {
                            UpdatePlayer(x);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void StopAnimation()
        {
            _gameViewTimer?.Dispose();
        }

        #endregion

        #region Car

        private void UpdateCar(GameObject car)
        {
            car.SetTop(car.GetTop() - car.Speed);

            if (car.GetTop() < 0 - car.Height)
            {
                RecyleCar(car);
            }
        }

        private void RecyleCar(GameObject car)
        {
            _markNum = _random.Next(0, _cars.Length);
            car.SetContent(_cars[_markNum]);
            car.SetSize(Constants.CAR_WIDTH * _scale, Constants.CAR_HEIGHT * _scale);
            car.Speed = _gameSpeed - _random.Next(1, 4);

            RandomizeCarPosition(car);
        }

        private void RandomizeCarPosition(GameObject car)
        {
            car.SetPosition(
                left: _random.Next(100, (int)UnderView.Width) - (100 * _scale),
                top: _random.Next((int)UnderView.Height, ((int)UnderView.Height) * 2));
        }

        #endregion

        #region Player

        private void UpdatePlayer(GameObject player)
        {
            player.SetTop(player.GetTop() - player.Speed);

            if (player.GetTop() < 0 - player.Height)
            {
                RecylePlayer(player);
            }
        }

        private void RecylePlayer(GameObject player)
        {
            player.SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key == ElementType.PLAYER).Value);
            player.SetSize(Constants.CAR_WIDTH * _scale, Constants.PLAYER_HEIGHT * _scale);
            player.Speed = _gameSpeed - _random.Next(1, 4);

            RandomizeCarPosition(player);
        }

        #endregion

        #region Cloud

        private void UpdateCloud(GameObject cloud)
        {
            cloud.SetTop(cloud.GetTop() + cloud.Speed);

            if (cloud.GetTop() > UnderView.Height)
            {
                RecyleCloud(cloud);
            }
        }

        private void RecyleCloud(GameObject cloud)
        {
            _markNum = _random.Next(0, _clouds.Length);

            cloud.SetContent(_clouds[_markNum]);
            cloud.SetSize(Constants.CLOUD_WIDTH * _scale, Constants.CLOUD_HEIGHT * _scale);
            cloud.Speed = _gameSpeed - _random.Next(1, 4);

            RandomizeCloudPosition(cloud);
        }

        private void RandomizeCloudPosition(GameObject cloud)
        {
            cloud.SetPosition(
                left: _random.Next(0, (int)UnderView.Width) - (100 * _scale),
                top: _random.Next(100 * (int)_scale, (int)UnderView.Height) * -1);
        }

        #endregion 

        #endregion

        #endregion
    }
}
