﻿using Microsoft.UI.Xaml;
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
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SkyWay
{
    public sealed partial class StartPage : Page
    {
        #region Fields

        private PeriodicTimer _gameViewTimer;
        private readonly TimeSpan _frameTime = TimeSpan.FromMilliseconds(18);

        private readonly Random _rand = new();

        private double _windowHeight, _windowWidth;
        private double _scale;

        private int _gameSpeed = 5;

        private int _markNum;

        private Uri[] _cars;
        private Uri[] _clouds;

        #endregion

        #region Ctor

        public StartPage()
        {
            this.InitializeComponent();

            _windowHeight = Window.Current.Bounds.Height;
            _windowWidth = Window.Current.Bounds.Width;

            LoadGameElements();
            SoundHelper.LoadGameSounds();
            InitializeGameViews();

            Loaded += GamePage_Loaded;
            Unloaded += GamePage_Unloaded;
        }

        #endregion

        #region Events

        #region Page

        private void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            SizeChanged += GamePage_SizeChanged;
            StartGame();
        }

        private void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= GamePage_SizeChanged;
            StopGame();
            StopGameSounds();
        }

        private void GamePage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            _windowWidth = args.NewSize.Width;
            _windowHeight = args.NewSize.Height;

            SetViewSize();

            Console.WriteLine($"WINDOWS SIZE: {_windowWidth}x{_windowHeight}");
        }

        #endregion

        #region Buttons

        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is string tag)
            {
                LocalizationHelper.CurrentCulture = tag;
                LocalizationHelper.SaveLocalizationCache(tag);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CookieAcceptButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CookieDeclineButton_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #endregion

        #region Methods

        #region Page

        private void SetViewSize()
        {
            _scale = ScalingHelper.GetGameObjectScale(_windowWidth);

            UnderView.Width = _windowWidth;
            UnderView.Height = _windowHeight;
        }

        #endregion

        #region Game

        private void InitializeGameViews()
        {
            Console.WriteLine("INITIALIZING GAME");

            SetViewSize();

            InitializeUnderView();
        }

        private void LoadGameElements()
        {
            _cars = Constants.ELEMENT_TEMPLATES.Where(x => x.Key == ElementType.CAR).Select(x => x.Value).ToArray();
            _clouds = Constants.ELEMENT_TEMPLATES.Where(x => x.Key == ElementType.CLOUD).Select(x => x.Value).ToArray();
        }

        private void InitializeUnderView()
        {
            // add some cars underneath
            for (int i = 0; i < 10; i++)
            {
                var car = new Car()
                {
                    Width = Constants.CAR_WIDTH * _scale,
                    Height = Constants.CAR_HEIGHT * _scale,
                    IsCollidable = false,
                    RenderTransform = new CompositeTransform()
                    {
                        ScaleX = 0.5,
                        ScaleY = 0.5,
                    }
                };

                RandomizeCarPosition(car);
                UnderView.Children.Add(car);
            }

            // add some clouds underneath
            for (int i = 0; i < 15; i++)
            {
                var scaleFactor = _rand.Next(1, 4);
                var scaleReverseFactor = _rand.Next(-1, 2);

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

        private void StartGame()
        {
            Console.WriteLine("GAME STARTED");

            StartGameSounds();

            RunGame();

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
        }

        private void StopGame()
        {
            _gameViewTimer.Dispose();
        }

        #endregion

        #region Car

        private void UpdateCar(GameObject car)
        {
            // move down vehicle
            car.SetTop(car.GetTop() + car.Speed);

            // if vechicle goes out of bounds
            if (car.GetTop() > UnderView.Height)
            {
                RecyleCar(car);
            }
        }

        private void RecyleCar(GameObject car)
        {
            _markNum = _rand.Next(0, _cars.Length);
            car.SetContent(_cars[_markNum]);
            car.SetSize(Constants.CAR_WIDTH * _scale, Constants.CAR_HEIGHT * _scale);
            car.Speed = _gameSpeed - _rand.Next(1, 4);

            RandomizeCarPosition(car);
        }

        private void RandomizeCarPosition(GameObject car)
        {
            car.SetPosition(
                left: _rand.Next(100, (int)UnderView.Width) - (100 * _scale),
                top: _rand.Next(100 * (int)_scale, (int)UnderView.Height) * -1);
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
            _markNum = _rand.Next(0, _clouds.Length);

            cloud.SetContent(_clouds[_markNum]);
            cloud.SetSize(Constants.CLOUD_WIDTH * _scale, Constants.CLOUD_HEIGHT * _scale);
            cloud.Speed = _gameSpeed - _rand.Next(1, 4);

            RandomizeCloudPosition(cloud);
        }

        private void RandomizeCloudPosition(GameObject cloud)
        {
            cloud.SetPosition(
                left: _rand.Next(0, (int)UnderView.Width) - (100 * _scale),
                top: _rand.Next(100 * (int)_scale, (int)UnderView.Height) * -1);
        }

        #endregion

        #region Sound

        private void StartGameSounds()
        {
            SoundHelper.RandomizeIntroSound();
            SoundHelper.PlaySound(SoundType.INTRO);
        }

        private void StopGameSounds()
        {
            SoundHelper.StopSound(SoundType.INTRO);
        }

        #endregion

        #endregion
    }
}