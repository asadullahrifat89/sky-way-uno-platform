﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

namespace SkyRacerGame
{
    public sealed partial class SignUpPage : Page
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

        #region Ctor

        public SignUpPage()
        {
            this.InitializeComponent();
            _backendService = (Application.Current as App).Host.Services.GetRequiredService<IBackendService>();

            _windowHeight = Window.Current.Bounds.Height;
            _windowWidth = Window.Current.Bounds.Width;

            LoadGameElements();
            PopulateGameViews();

            this.Loaded += SignUpPage_Loaded;
            this.Unloaded += SignUpPage_Unloaded;
        }

        #endregion

        #region Events

        #region Page

        private void SignUpPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetLocalization();

            SizeChanged += GamePage_SizeChanged;
            StartAnimation();
        }

        private void SignUpPage_Unloaded(object sender, RoutedEventArgs e)
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

        private async void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            if (SignupButton.IsEnabled)
                await PerformSignup();
        }

        private void LoginToExistingAccountButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(LoginPage));
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(StartPage));
        }

        #endregion

        #region Input Fields

        private void UserFullNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSignupButton();
        }

        private void UserEmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSignupButton();
        }

        private void UserNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSignupButton();
        }

        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSignupButton();
        }

        private async void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && SignupButton.IsEnabled)
                await PerformSignup();
        }

        private void ConfirmPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSignupButton();
        }

        private void ConfirmCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableSignupButton();
        }

        private void ConfirmCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableSignupButton();
        }

        #endregion

        #endregion

        #region Methods

        #region Logic

        private async Task PerformSignup()
        {
            this.RunProgressBar();

            if (await Signup() && await Authenticate())
            {
                this.StopProgressBar();
                NavigateToPage(typeof(LoginPage));
            }
        }

        private async Task<bool> Signup()
        {
            (bool IsSuccess, string Message) = await _backendService.SignupUser(
                fullName: UserFullNameBox.Text.Trim(),
                userName: UserNameBox.Text.Trim(),
                email: UserEmailBox.Text.ToLower().Trim(),
                password: PasswordBox.Text.Trim());

            if (!IsSuccess)
            {
                var error = Message;
                this.ShowError(error);
                return false;
            }

            return true;
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

        private void EnableSignupButton()
        {
            SignupButton.IsEnabled =
                !UserFullNameBox.Text.IsNullOrBlank()
                && IsValidFullName()
                && IsStrongPassword()
                && DoPasswordsMatch()
                && !UserNameBox.Text.IsNullOrBlank()
                && !UserEmailBox.Text.IsNullOrBlank()
                && IsValidEmail()
                && ConfirmCheckBox.IsChecked == true;
        }

        private bool IsValidFullName()
        {
            (bool IsValid, string Message) = StringExtensions.IsValidFullName(UserFullNameBox.Text);
            if (!IsValid)
                this.SetProgressBarMessage(message: LocalizationHelper.GetLocalizedResource(Message), isError: true);
            else
                ProgressBarMessageBlock.Visibility = Visibility.Collapsed;

            return IsValid;
        }

        private bool IsStrongPassword()
        {
            (bool IsStrong, string Message) = StringExtensions.IsStrongPassword(PasswordBox.Text);
            this.SetProgressBarMessage(message: LocalizationHelper.GetLocalizedResource(Message), isError: !IsStrong);

            return IsStrong;
        }

        private bool DoPasswordsMatch()
        {
            if (PasswordBox.Text.IsNullOrBlank() || ConfirmPasswordBox.Text.IsNullOrBlank())
                return false;

            if (PasswordBox.Text != ConfirmPasswordBox.Text)
            {
                this.SetProgressBarMessage(message: LocalizationHelper.GetLocalizedResource("PASSWORDS_DIDNT_MATCH"), isError: true);
                return false;
            }
            else
            {
                this.SetProgressBarMessage(message: LocalizationHelper.GetLocalizedResource("PASSWORDS_MATCHED"), isError: false);
            }

            return true;
        }

        private bool IsValidEmail()
        {
            return StringExtensions.IsValidEmail(UserEmailBox.Text);
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