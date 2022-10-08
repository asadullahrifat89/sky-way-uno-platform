using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;

namespace SkyWay
{
    public sealed partial class GamePage : Page
    {
        #region Fields

        private PeriodicTimer _gameViewTimer;

        private readonly Random _rand = new();

        private Rect _playerHitBox;

        private int _gameSpeed;
        private readonly int _defaultGameSpeed = 5;
        private readonly int _playerSpeed = 9;
        private int _markNum;

        private int _powerUpSpawnCounter = 30;

        private int _powerModeCounter = 250;
        private readonly int _powerModeDelay = 250;

        private int _lives;
        private readonly int _maxLives = 5;
        private int _healthSpawnCounter = 500;
        int _damageRecoveryOpacityFrameSkip;

        private int _collectibleSpawnCounter = 200;

        private double _score;
        private int _collectiblesCollected;

        private int _islandSpawnCounter;

        private bool _moveLeft;
        private bool _moveRight;
        private bool _moveUp;
        private bool _moveDown;
        private bool _isGameOver;
        private bool _isPowerMode;
        private bool _isGamePaused;
        private bool _isGameQuitting;

        private bool _isRecoveringFromDamage;
        private bool _isPointerActivated;
        private readonly TimeSpan _frameTime = TimeSpan.FromMilliseconds(Constants.DEFAULT_FRAME_TIME);

        private int _accelerationCounter;

        private int _damageRecoveryCounter = 100;
        private readonly int _damageRecoveryDelay = 500;

        private double _windowHeight, _windowWidth;
        private double _scale;
        private Point _pointerPosition;

        private Player _player;

        private Uri[] _cars;
        private Uri[] _islands;
        private Uri[] _clouds;

        #endregion

        #region Ctor

        public GamePage()
        {
            InitializeComponent();

            _isGameOver = true;

            _windowHeight = Window.Current.Bounds.Height;
            _windowWidth = Window.Current.Bounds.Width;

            LoadGameElements();
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
        }

        private void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= GamePage_SizeChanged;
        }

        private void GamePage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            _windowWidth = args.NewSize.Width;
            _windowHeight = args.NewSize.Height;

            SetViewSize();

            Console.WriteLine($"WINDOWS SIZE: {_windowWidth}x{_windowHeight}");
        }

        #endregion

        #region Input

        private void InputView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_isGameOver)
            {
                InputView.Focus(FocusState.Programmatic);
                StartGame();
            }
            else
            {
                _isPointerActivated = true;
            }
        }

        private void InputView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isPointerActivated)
            {
                PointerPoint point = e.GetCurrentPoint(GameView);
                _pointerPosition = point.Position;
            }
        }

        private void InputView_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isPointerActivated = false;
            _pointerPosition = null;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Left)
            {
                _moveLeft = true;
                _moveRight = false;
            }
            if (e.Key == VirtualKey.Right)
            {
                _moveRight = true;
                _moveLeft = false;
            }
            if (e.Key == VirtualKey.Up)
            {
                _moveUp = true;
                _moveDown = false;
            }
            if (e.Key == VirtualKey.Down)
            {
                _moveDown = true;
                _moveUp = false;
            }
        }

        private void OnKeyUP(object sender, KeyRoutedEventArgs e)
        {
            // when the player releases the left or right key it will set the designated boolean to false
            if (e.Key == VirtualKey.Left)
            {
                _moveLeft = false;
            }
            if (e.Key == VirtualKey.Right)
            {
                _moveRight = false;
            }
            if (e.Key == VirtualKey.Up)
            {
                _moveUp = false;
            }
            if (e.Key == VirtualKey.Down)
            {
                _moveDown = false;
            }

            if (!_moveLeft && !_moveRight && !_moveUp && !_moveDown)
                _accelerationCounter = 0;

            // in this case we will listen for the enter key aswell but for this to execute we will need the game over boolean to be true
            if (e.Key == VirtualKey.Enter && _isGameOver == true)
            {
                StartGame();
            }
        }

        #endregion

        #region Game

        private void PauseGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }

        private void QuitGameButton_Click(object sender, RoutedEventArgs e)
        {
            QuitGame();
        }

        #endregion

        #endregion

        #region Methods

        #region Game

        private void InitializeGameViews()
        {
            Console.WriteLine("INITIALIZING GAME");

            SetViewSize();

            InitializeUnderView();
            InitializeGameView();
            InitializeOverView();
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

        private void InitializeGameView()
        {
            // add 5 cars
            for (int i = 0; i < 5; i++)
            {
                var car = new Car()
                {
                    Width = Constants.CAR_WIDTH * _scale,
                    Height = Constants.CAR_HEIGHT * _scale,
                    IsCollidable = true,
                };

                RandomizeCarPosition(car);
                GameView.Children.Add(car);
            }

            // add player
            _player = new Player()
            {
                Width = Constants.PLAYER_WIDTH * _scale,
                Height = Constants.PLAYER_HEIGHT * _scale,
            };

            _player.SetPosition(
                left: GameView.Width / 2 - _player.Width / 2,
                top: GameView.Height - _player.Height - (50 * _scale));

            GameView.Children.Add(_player);
        }

        private void InitializeOverView()
        {
            // add some clouds above
            for (int i = 0; i < 5; i++)
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
                OverView.Children.Add(cloud);
            }
        }

        private void LoadGameElements()
        {
            _cars = Constants.ELEMENT_TEMPLATES.Where(x => x.Key == ElementType.CAR).Select(x => x.Value).ToArray();
            _islands = Constants.ELEMENT_TEMPLATES.Where(x => x.Key == ElementType.ISLAND).Select(x => x.Value).ToArray();
            _clouds = Constants.ELEMENT_TEMPLATES.Where(x => x.Key == ElementType.CLOUD).Select(x => x.Value).ToArray();
        }

        private void StartGame()
        {
            Console.WriteLine("GAME STARTED");

            _lives = _maxLives;
            SetLives();

            _gameSpeed = _defaultGameSpeed;
            _player.Opacity = 1;

            ResetControls();

            _isGameOver = false;
            _isPowerMode = false;
            _powerModeCounter = _powerModeDelay;
            _isRecoveringFromDamage = false;
            _damageRecoveryCounter = _damageRecoveryDelay;

            _score = 0;
            _collectiblesCollected = 0;
            scoreText.Text = "0";

            foreach (GameObject x in SeaView.Children.OfType<GameObject>())
            {
                SeaView.AddDestroyableGameObject(x);
            }

            RecycleGameObjects();
            RemoveGameObjects();          

            StartGameSounds();
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

            // remove health and power ups, recylce cars
            foreach (GameObject x in GameView.Children.OfType<GameObject>())
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
                    case ElementType.COLLECTIBLE:
                    case ElementType.HEALTH:
                    case ElementType.POWERUP:
                        {
                            GameView.AddDestroyableGameObject(x);
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
                    case ElementType.CLOUD:
                        {
                            RecyleCloud(x);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ResetControls()
        {
            _moveLeft = false;
            _moveRight = false;
            _moveUp = false;
            _moveDown = false;
            _isPointerActivated = false;
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
            _score += .05; // increase the score by .5 each tick of the timer
            scoreText.Text = _score.ToString("#");

            _playerHitBox = _player.GetHitBox(_scale);

            SpawnGameObjects();
            UpdateGameObjects();
            RemoveGameObjects();

            if (_isGameOver)
                return;

            if (_isPowerMode)
            {
                PowerUpCoolDown();

                if (_powerModeCounter <= 0)
                {
                    PowerDown();
                }
            }

            // as you progress in the game you will score higher and game speed will go up
            ScaleDifficulty();
        }

        private void SpawnGameObjects()
        {
            _powerUpSpawnCounter--;
            _collectibleSpawnCounter--;
            _islandSpawnCounter--;

            if (_powerUpSpawnCounter < 1)
            {
                SpawnPowerUp();
                _powerUpSpawnCounter = _rand.Next(500, 800);
            }

            if (_collectibleSpawnCounter < 1)
            {
                SpawnCollectible();
                _collectibleSpawnCounter = _rand.Next(200, 300);
            }

            if (_islandSpawnCounter < 1)
            {
                SpawnIsland();
                _islandSpawnCounter = _rand.Next(1500, 2000);
            }

            if (_lives < _maxLives)
            {
                _healthSpawnCounter--;

                if (_healthSpawnCounter < 0)
                {
                    SpawnHealth();
                    _healthSpawnCounter = _rand.Next(500, 800);
                }
            }
        }

        private void UpdateGameObjects()
        {
            foreach (GameObject x in SeaView.Children.OfType<GameObject>())
            {
                UpdateIsland(x);
            }

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

            foreach (GameObject x in GameView.Children.OfType<GameObject>())
            {
                switch ((ElementType)x.Tag)
                {
                    case ElementType.CAR:
                        {
                            UpdateCar(x);
                        }
                        break;
                    case ElementType.POWERUP:
                        {
                            UpdatePowerUp(x);
                        }
                        break;
                    case ElementType.COLLECTIBLE:
                        {
                            UpdateCollectible(x);
                        }
                        break;
                    case ElementType.HEALTH:
                        {
                            UpdateHealth(x);
                        }
                        break;
                    //case Constants.ROADMARK:
                    //    {
                    //        UpdateRoadMark(x);
                    //    }
                    //    break;
                    case ElementType.PLAYER:
                        {
                            if (_moveLeft || _moveRight || _moveUp || _moveDown || _isPointerActivated)
                            {
                                UpdatePlayer();
                            }
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

        private void RemoveGameObjects()
        {
            SeaView.RemoveDestroyableGameObjects();
            UnderView.RemoveDestroyableGameObjects();
            GameView.RemoveDestroyableGameObjects();
            OverView.RemoveDestroyableGameObjects();
        }

        private void GameOver()
        {
            StopGame();
            StopGameSounds();

            _isGameOver = true;

            PlayerScoreHelper.PlayerScore = new SkyWayScore()
            {
                Score = Math.Ceiling(_score),
                CollectiblesCollected = _collectiblesCollected
            };

            SoundHelper.PlaySound(SoundType.GAME_OVER);
            NavigateToPage(typeof(GameOverPage));
        }

        private void PauseGame()
        {
            _isGamePaused = true;

            StopGame();
            ResetControls();

            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            SoundHelper.PauseSound(SoundType.BACKGROUND);
            SoundHelper.PauseSound(SoundType.CAR_ENGINE);

            InputView.Focus(FocusState.Programmatic);
        }

        private void ResumeGame()
        {
            InputView.Focus(FocusState.Programmatic);

            _isGamePaused = false;
            _isGameQuitting = false;

            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            SoundHelper.ResumeSound(SoundType.BACKGROUND);
            SoundHelper.ResumeSound(SoundType.CAR_ENGINE);

            RunGame();
        }

        private void StopGame()
        {
            _gameViewTimer?.Dispose();
        }

        private void QuitGame()
        {
            if (_isGameQuitting)
            {
                StopGame();
                StopGameSounds();
                NavigateToPage(typeof(StartPage));
            }
            else
            {
                _isGameQuitting = true;
                PauseGame();
            }

            //TODO: show game quitting content
        }

        #endregion

        #region Car

        private void UpdateCar(GameObject car)
        {
            // move down vehicle
            car.SetTop(car.GetTop() + car.Speed);

            // if vechicle goes out of bounds
            if (car.GetTop() > GameView.Height)
            {
                RecyleCar(car);
            }

            if (car.IsCollidable)
            {
                if (_isRecoveringFromDamage)
                {
                    _damageRecoveryOpacityFrameSkip--;
                    if (_damageRecoveryOpacityFrameSkip < 0)
                    {
                        _player.Opacity = 0.33;
                        _damageRecoveryOpacityFrameSkip = 5;
                    }
                    else
                    {
                        _player.Opacity = 1;
                    }

                    _damageRecoveryCounter--;

                    if (_damageRecoveryCounter <= 0)
                    {
                        _player.Opacity = 1;
                        _isRecoveringFromDamage = false;
                    }
                }
                else
                {
                    // if car collides with player
                    if (_playerHitBox.IntersectsWith(car.GetHitBox(_scale)))
                    {
                        if (!_isPowerMode)
                        {
                            _lives--;
                            _damageRecoveryCounter = _damageRecoveryDelay;
                            _isRecoveringFromDamage = true;
                            SetLives();
                            SoundHelper.PlaySound(SoundType.HEALTH_LOSS);

                            if (_lives == 0)
                                GameOver();
                        }
                    }
                }
            }

            if (_isGameOver)
                return;
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
                left: _rand.Next(100, (int)GameView.Width) - (100 * _scale),
                top: _rand.Next(100 * (int)_scale, (int)GameView.Height) * -1);
        }

        #endregion

        #region Cloud

        private void UpdateCloud(GameObject cloud)
        {
            cloud.SetTop(cloud.GetTop() + cloud.Speed);

            if (cloud.GetTop() > GameView.Height)
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
                left: _rand.Next(0, (int)GameView.Width) - (100 * _scale),
                top: _rand.Next(100 * (int)_scale, (int)GameView.Height) * -1);
        }

        #endregion

        #region Collectible

        private void SpawnCollectible()
        {
            double top = GameView.Height * -1;
            double left = _rand.Next(0, (int)(GameView.Width - 55));

            double xDir = _rand.Next(-1, 2);

            for (int i = 0; i < 5; i++)
            {
                Collectible collectible = new()
                {
                    Height = Constants.COLLECTIBLE_HEIGHT * _scale,
                    Width = Constants.COLLECTIBLE_WIDTH * _scale,
                    Speed = _gameSpeed - _gameSpeed / 2,
                };

                collectible.SetPosition(left: left, top: top);
                GameView.Children.Add(collectible);

                switch (xDir)
                {
                    case -1:
                        {
                            left -= collectible.Width;
                        }
                        break;
                    case 1:
                        {
                            left += collectible.Width;
                        }
                        break;
                    default:
                        break;
                }

                top += collectible.Height;
            }
        }

        private void UpdateCollectible(GameObject collectible)
        {
            collectible.SetTop(collectible.GetTop() + collectible.Speed);

            if (_playerHitBox.IntersectsWith(collectible.GetHitBox(_scale)))
            {
                GameView.AddDestroyableGameObject(collectible);
                Collectible();
            }

            if (collectible.GetTop() > GameView.Height)
            {
                GameView.AddDestroyableGameObject(collectible);
            }
        }

        private void Collectible()
        {
            _score++;
            _collectiblesCollected++;
            SoundHelper.PlaySound(SoundType.COLLECTIBLE_COLLECTED);
        }

        #endregion        

        #region Island

        private void SpawnIsland()
        {
            var island = new Island()
            {
                Width = Constants.ISLAND_WIDTH * _scale,
                Height = Constants.ISLAND_HEIGHT * _scale,
            };

            island.RenderTransform = new CompositeTransform()
            {
                CenterX = 0.5,
                CenterY = 0.5,
                Rotation = _rand.Next(0, 360),
            };

            _markNum = _rand.Next(0, _islands.Length);
            island.SetContent(_islands[_markNum]);

            RandomizeIslandPosition(island);
            SeaView.Children.Add(island);

            Console.WriteLine($"ISLAND SPAWN: X={island.GetLeft()} Y={island.GetTop()}");
        }

        private void UpdateIsland(GameObject island)
        {
            island.SetTop(island.GetTop() + _gameSpeed / 6);

            if (island.GetTop() > SeaView.Height)
            {
                SeaView.AddDestroyableGameObject(island);

            }
        }

        private void RandomizeIslandPosition(GameObject island)
        {
            island.SetPosition(
                left: _rand.Next(0, (int)GameView.Width) - 100 * _scale,
                top: _rand.Next(0, (int)GameView.Height) * -1);
        }

        #endregion      

        #region Power Up

        private void SpawnPowerUp()
        {
            PowerUp powerUp = new()
            {
                Height = Constants.POWERUP_HEIGHT * _scale,
                Width = Constants.POWERUP_WIDTH * _scale,
            };

            powerUp.SetPosition(
                left: _rand.Next(0, (int)(GameView.Width - 55)),
                top: _rand.Next(100, (int)GameView.Height) * -1);

            GameView.Children.Add(powerUp);
        }

        private void UpdatePowerUp(GameObject powerUp)
        {
            powerUp.SetTop(powerUp.GetTop() + 5);

            if (_playerHitBox.IntersectsWith(powerUp.GetHitBox(_scale)))
            {
                GameView.AddDestroyableGameObject(powerUp);
                PowerUp();
            }

            if (powerUp.GetTop() > GameView.Height)
            {
                GameView.AddDestroyableGameObject(powerUp);
            }
        }

        private void PowerUp()
        {
            powerUpText.Visibility = Visibility.Visible;

            _isPowerMode = true;

            _powerModeCounter = _powerModeDelay;

            _player.SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key == ElementType.PLAYER_POWER_MODE).Value);

            SoundHelper.PlaySound(SoundType.POWER_UP);
        }

        private void PowerUpCoolDown()
        {
            _powerModeCounter -= 1;

            double remainingPow = (double)_powerModeCounter / (double)_powerModeDelay * 4;

            powerUpText.Text = "";
            for (int i = 0; i < remainingPow; i++)
            {
                powerUpText.Text += "⚡";
            }
        }

        private void PowerDown()
        {
            _isPowerMode = false;

            powerUpText.Visibility = Visibility.Collapsed;
            _player.SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is ElementType.PLAYER).Value);
            SoundHelper.PlaySound(SoundType.POWER_DOWN);
        }

        #endregion

        #region Health

        private void SetLives()
        {
            livesText.Text = "";
            for (int i = 0; i < _lives; i++)
            {
                livesText.Text += "❤️";
            }
        }

        private void SpawnHealth()
        {
            Health health = new()
            {
                Height = Constants.HEALTH_HEIGHT * _scale,
                Width = Constants.HEALTH_WIDTH * _scale,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { Angle = Convert.ToDouble(Resources["FoliageViewRotationAngle"]) },
            };

            health.SetPosition(
                left: _rand.Next(0, (int)(GameView.Width - 55)),
                top: _rand.Next(100, (int)GameView.Height) * -1);

            GameView.Children.Add(health);
        }

        private void UpdateHealth(GameObject health)
        {
            health.SetTop(health.GetTop() + 5);

            // if player gets a health
            if (_playerHitBox.IntersectsWith(health.GetHitBox(_scale)))
            {
                GameView.AddDestroyableGameObject(health);
                Health();
            }

            if (health.GetTop() > GameView.Height)
            {
                GameView.AddDestroyableGameObject(health);
            }
        }

        private void Health()
        {
            _lives++;
            SetLives();
            SoundHelper.PlaySound(SoundType.HEALTH_GAIN);
        }

        #endregion

        #region Player

        private void UpdatePlayer()
        {
            double effectiveSpeed = _accelerationCounter >= _playerSpeed ? _playerSpeed : _accelerationCounter / 1.3;

            // increase acceleration and stop when player speed is reached
            if (_accelerationCounter <= _playerSpeed)
                _accelerationCounter++;

            //Console.WriteLine("ACC:" + _accelerationCounter);            

            double left = _player.GetLeft();
            double top = _player.GetTop();

            double playerMiddleX = left + _player.Width / 2;
            double playerMiddleY = top + _player.Height / 2;

            if (_isPointerActivated)
            {
                // move up
                if (_pointerPosition.Y < playerMiddleY - _playerSpeed)
                {
                    _player.SetTop(top - effectiveSpeed);
                }
                // move left
                if (_pointerPosition.X < playerMiddleX - _playerSpeed && left > 0)
                {
                    _player.SetLeft(left - effectiveSpeed);
                }

                // move down
                if (_pointerPosition.Y > playerMiddleY + _playerSpeed)
                {
                    _player.SetTop(top + effectiveSpeed);
                }
                // move right
                if (_pointerPosition.X > playerMiddleX + _playerSpeed && left + _player.Width < GameView.Width)
                {
                    _player.SetLeft(left + effectiveSpeed);
                }
            }
            else
            {
                if (_moveLeft && left > 0)
                {
                    _player.SetLeft(left - effectiveSpeed);
                }
                if (_moveRight && left + _player.Width < GameView.Width)
                {
                    _player.SetLeft(left + effectiveSpeed);
                }
                if (_moveUp && top > 0 + (50 * _scale))
                {
                    _player.SetTop(top - effectiveSpeed);
                }
                if (_moveDown && top < GameView.Height - (100 * _scale))
                {
                    _player.SetTop(top + effectiveSpeed);
                }
            }
        }

        #endregion

        #region Road Marks

        private void UpdateRoadMark(GameObject roadMark)
        {
            roadMark.SetTop(roadMark.GetTop() + _gameSpeed);

            if (roadMark.GetTop() > GameView.Height)
            {
                RecyleRoadMark(roadMark);
            }
        }

        private void RecyleRoadMark(GameObject roadMark)
        {
            roadMark.SetSize(Constants.ROADMARK_WIDTH * _scale, Constants.ROADMARK_HEIGHT * _scale);
            roadMark.SetTop((int)roadMark.Height * 2 * -25);
        }

        #endregion

        #region Game Difficulty

        private void ScaleDifficulty()
        {
            if (_score >= 10 && _score < 20)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 1;
            }

            if (_score >= 20 && _score < 30)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 2;
            }
            if (_score >= 30 && _score < 40)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 3;
            }
            if (_score >= 40 && _score < 50)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 4;
            }
            if (_score >= 50 && _score < 80)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 5;
            }
            if (_score >= 80 && _score < 100)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 6;
            }
            if (_score >= 100 && _score < 130)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 7;
            }
            if (_score >= 130 && _score < 150)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 8;
            }
            if (_score >= 150 && _score < 180)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 9;
            }
            if (_score >= 180 && _score < 200)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 10;
            }
        }

        #endregion

        #region Sound

        private async void StartGameSounds()
        {
            SoundHelper.PlaySound(SoundType.CAR_START);

            await Task.Delay(TimeSpan.FromSeconds(1));

            SoundHelper.PlaySound(SoundType.CAR_ENGINE);

            SoundHelper.RandomizeBackgroundSound();
            SoundHelper.PlaySound(SoundType.BACKGROUND);
        }

        private void StopGameSounds()
        {
            SoundHelper.StopSound(SoundType.BACKGROUND);
            SoundHelper.StopSound(SoundType.CAR_ENGINE);
        }

        #endregion

        #region Page

        private void SetViewSize()
        {
            _scale = ScalingHelper.GetGameObjectScale(_windowWidth);

            SeaView.Width = _windowWidth;
            SeaView.Height = _windowHeight;

            UnderView.Width = _windowWidth;
            UnderView.Height = _windowHeight;

            GameView.Width = _windowWidth;
            GameView.Height = _windowHeight;

            OverView.Width = _windowWidth;
            OverView.Height = _windowHeight;
        }

        private void NavigateToPage(Type pageType)
        {
            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            App.NavigateToPage(pageType);
        }

        #endregion

        #endregion
    }
}
