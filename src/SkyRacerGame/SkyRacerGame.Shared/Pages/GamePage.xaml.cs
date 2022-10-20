using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;

namespace SkyRacerGame
{
    public sealed partial class GamePage : Page
    {
        #region Fields

        private PeriodicTimer _gameViewTimer;
        private readonly TimeSpan _frameTime = TimeSpan.FromMilliseconds(Constants.DEFAULT_FRAME_TIME);

        private readonly Random _random = new();

        private Rect _playerHitBox;

        private int _gameSpeed;
        private readonly int _defaultGameSpeed = 5;

        private int _playerSpeed = 9;
        private int _defaultPlayerSpeed = 9;
        private int _markNum;

        private int _powerUpSpawnCounter = 30;

        private int _powerModeCounter = 500;
        private readonly int _powerModeDelay = 500;

        private int _lives;
        private readonly int _maxLives = 3;
        private int _healthSpawnCounter = 500;
        private int _damageRecoveryOpacityFrameSkip;

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

        private bool _isRecoveringFromDamage;
        private bool _isPointerActivated;

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

        private PowerUpType _powerUpType;

        #endregion

        #region Ctor

        public GamePage()
        {
            InitializeComponent();

            _isGameOver = true;
            ShowInGameTextMessage("TAP_ON_SCREEN_TO_BEGIN");

            _windowHeight = Window.Current.Bounds.Height;
            _windowWidth = Window.Current.Bounds.Width;

            LoadGameElements();
            PopulateGameViews();

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
            StopGame();
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
        }

        #endregion

        #region Button

        private void QuitGameButton_Checked(object sender, RoutedEventArgs e)
        {
            PauseGame();
        }

        private void QuitGameButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ResumeGame();
        }

        private void ConfirmQuitGameButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(typeof(StartPage));
        }

        #endregion

        #endregion

        #region Methods

        #region Animation

        #region Game

        private void PopulateGameViews()
        {
#if DEBUG
            Console.WriteLine("INITIALIZING GAME");
#endif

            SetViewSize();

            PopulateUnderView();
            PopulateGameView();
            PopulateOverView();
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

        private void PopulateGameView()
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

        private void PopulateOverView()
        {
            // add some clouds above
            for (int i = 0; i < 5; i++)
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
#if DEBUG
            Console.WriteLine("GAME STARTED");
#endif
            HideInGameTextMessage();
            SoundHelper.PlaySound(SoundType.MENU_SELECT);

            _lives = _maxLives;
            SetLives();

            _gameSpeed = _defaultGameSpeed;
            _playerSpeed = _defaultPlayerSpeed;
            _player.Opacity = 1;

            ResetControls();

            _isGameOver = false;
            _isPowerMode = false;
            _powerUpType = 0;
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

        private async void RunGame()
        {
            _gameViewTimer = new PeriodicTimer(_frameTime);

            while (await _gameViewTimer.WaitForNextTickAsync())
            {
                GameViewLoop();
            }
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

        private void GameViewLoop()
        {
            AddScore(0.05d); // increase the score by .5 each tick of the timer
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
                    PowerDown();
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
                _powerUpSpawnCounter = _random.Next(500, 800);
            }

            if (_collectibleSpawnCounter < 1)
            {
                SpawnCollectible();
                _collectibleSpawnCounter = _random.Next(200, 300);
            }

            if (_islandSpawnCounter < 1)
            {
                SpawnIsland();
                _islandSpawnCounter = _random.Next(1500, 2000);
            }

            if (_lives < _maxLives)
            {
                _healthSpawnCounter--;

                if (_healthSpawnCounter < 0)
                {
                    SpawnHealth();
                    _healthSpawnCounter = _random.Next(500, 800);
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

        private void PauseGame()
        {
            InputView.Focus(FocusState.Programmatic);
            ShowInGameTextMessage("GAME_PAUSED");

            _gameViewTimer?.Dispose();

            ResetControls();

            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            PauseGameSounds();
        }

        private void ResumeGame()
        {
            InputView.Focus(FocusState.Programmatic);
            HideInGameTextMessage();

            SoundHelper.PlaySound(SoundType.MENU_SELECT);
            SoundHelper.ResumeSound(SoundType.BACKGROUND);
            SoundHelper.ResumeSound(SoundType.CAR_ENGINE);

            RunGame();
        }

        private void StopGame()
        {
            _gameViewTimer?.Dispose();
            StopGameSounds();
        }

        private void GameOver()
        {
            _isGameOver = true;

            PlayerScoreHelper.PlayerScore = new SkyRacerGameScore()
            {
                Score = Math.Ceiling(_score),
                CollectiblesCollected = _collectiblesCollected
            };

            SoundHelper.PlaySound(SoundType.GAME_OVER);
            NavigateToPage(typeof(GameOverPage));
        }

        private double DecreaseSpeed(double speed)
        {
            if (_isPowerMode && _powerUpType == PowerUpType.SLOW_DOWN_TIME)
                speed /= 3;

            return speed;
        }

        #endregion

        #region Car

        private void UpdateCar(GameObject car)
        {
            var speed = car.Speed;
            speed = DecreaseSpeed(speed);

            // move down vehicle
            car.SetTop(car.GetTop() + speed);

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
                    if (!_isPowerMode || _powerUpType != PowerUpType.FORCE_SHIELD)
                    {
                        // if car collides with player
                        if (_playerHitBox.IntersectsWith(car.GetHitBox(_scale)))
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
        }

        private void RecyleCar(GameObject car)
        {
            var speed = (double)_gameSpeed - (double)_random.Next(1, 4);

            _markNum = _random.Next(0, _cars.Length);
            car.SetContent(_cars[_markNum]);
            car.SetSize(Constants.CAR_WIDTH * _scale, Constants.CAR_HEIGHT * _scale);
            car.Speed = speed;

            RandomizeCarPosition(car);
        }

        private void RandomizeCarPosition(GameObject car)
        {
            car.SetPosition(
                left: _random.Next(100, (int)GameView.Width) - (100 * _scale),
                top: _random.Next(100 * (int)_scale, (int)GameView.Height) * -1);
        }

        #endregion

        #region Cloud

        private void UpdateCloud(GameObject cloud)
        {
            var speed = cloud.Speed;
            speed = DecreaseSpeed(speed);

            cloud.SetTop(cloud.GetTop() + speed);

            if (cloud.GetTop() > GameView.Height)
            {
                RecyleCloud(cloud);
            }
        }

        private void RecyleCloud(GameObject cloud)
        {
            var speed = (double)_gameSpeed - (double)_random.Next(1, 4);

            _markNum = _random.Next(0, _clouds.Length);

            cloud.SetContent(_clouds[_markNum]);
            cloud.SetSize(Constants.CLOUD_WIDTH * _scale, Constants.CLOUD_HEIGHT * _scale);
            cloud.Speed = speed;

            RandomizeCloudPosition(cloud);
        }

        private void RandomizeCloudPosition(GameObject cloud)
        {
            cloud.SetPosition(
                left: _random.Next(0, (int)GameView.Width) - (100 * _scale),
                top: _random.Next(100 * (int)_scale, (int)GameView.Height) * -1);
        }

        #endregion

        #region Collectible

        private void SpawnCollectible()
        {
            double top = GameView.Height * -1;
            double left = _random.Next(0, (int)(GameView.Width - 55));

            double xDir = _random.Next(-1, 2);

            var speed = (double)_gameSpeed - (double)_gameSpeed / 2;

            for (int i = 0; i < 5; i++)
            {
                Collectible collectible = new()
                {
                    Height = Constants.COLLECTIBLE_HEIGHT * _scale,
                    Width = Constants.COLLECTIBLE_WIDTH * _scale,
                    Speed = speed,
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

                if (left < 0)
                    left = 0;

                if (left + collectible.Width >= GameView.Width)
                    left = GameView.Width - collectible.Width;

                top += collectible.Height;
            }
        }

        private void UpdateCollectible(GameObject collectible)
        {
            var speed = collectible.Speed;
            speed = DecreaseSpeed(speed);

            collectible.SetTop(collectible.GetTop() + speed);

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
            AddScore(1); // increase the score by 1 if collectible is collected
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
                Rotation = _random.Next(0, 360),
            };

            _markNum = _random.Next(0, _islands.Length);
            island.SetContent(_islands[_markNum]);

            RandomizeIslandPosition(island);
            SeaView.Children.Add(island);

            Console.WriteLine($"ISLAND SPAWN: X={island.GetLeft()} Y={island.GetTop()}");
        }

        private void UpdateIsland(GameObject island)
        {
            var speed = (double)_gameSpeed / 6;
            speed = DecreaseSpeed(speed);

            island.SetTop(island.GetTop() + speed);

            if (island.GetTop() > SeaView.Height)
            {
                SeaView.AddDestroyableGameObject(island);
            }
        }

        private void RandomizeIslandPosition(GameObject island)
        {
            island.SetPosition(
                left: _random.Next(0, (int)GameView.Width) - 100 * _scale,
                top: _random.Next(0, (int)GameView.Height) * -1);
        }

        #endregion      

        #region PowerUp

        private void SpawnPowerUp()
        {
            var _markNum = _random.Next(1, Enum.GetNames<PowerUpType>().Length);
            var powerUpTemplates = Constants.ELEMENT_TEMPLATES.Where(x => x.Key is ElementType.POWERUP).ToArray();

            PowerUp powerUp = new()
            {
                Height = Constants.POWERUP_HEIGHT * _scale,
                Width = Constants.POWERUP_WIDTH * _scale,
                PowerUpType = (PowerUpType)_markNum,
            };

            powerUp.SetContent(powerUpTemplates[_markNum - 1].Value);

            powerUp.SetPosition(
                left: _random.Next(0, (int)(GameView.Width - 55)),
                top: _random.Next(100, (int)GameView.Height) * -1);

            GameView.Children.Add(powerUp);
        }

        private void UpdatePowerUp(GameObject powerUp)
        {
            powerUp.SetTop(powerUp.GetTop() + 5);

            if (_playerHitBox.IntersectsWith(powerUp.GetHitBox(_scale)))
            {
                GameView.AddDestroyableGameObject(powerUp);
                PowerUp(powerUp as PowerUp);
            }

            if (powerUp.GetTop() > GameView.Height)
            {
                GameView.AddDestroyableGameObject(powerUp);
            }
        }

        private void PowerUp(PowerUp powerUp)
        {
            powerUpText.Visibility = Visibility.Visible;

            _isPowerMode = true;
            _powerUpType = powerUp.PowerUpType;
            _powerModeCounter = _powerModeDelay;

            if (_powerUpType == PowerUpType.FORCE_SHIELD)
                _player.SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key == ElementType.PLAYER_POWER_MODE).Value);
            else
                _player.SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is ElementType.PLAYER).Value);

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
            _powerUpType = 0;

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
                left: _random.Next(0, (int)(GameView.Width - 55)),
                top: _random.Next(100, (int)GameView.Height) * -1);

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

            double left = _player.GetLeft();
            double top = _player.GetTop();

            double playerMiddleX = left + _player.Width / 2;
            double playerMiddleY = top + _player.Height / 2;

            if (_isPointerActivated)
            {
                // move up
                if (_pointerPosition.Y < playerMiddleY - _playerSpeed)
                    _player.SetTop(top - effectiveSpeed);

                // move left
                if (_pointerPosition.X < playerMiddleX - _playerSpeed && left > 0)
                    _player.SetLeft(left - effectiveSpeed);

                // move down
                if (_pointerPosition.Y > playerMiddleY + _playerSpeed)
                    _player.SetTop(top + effectiveSpeed);

                // move right
                if (_pointerPosition.X > playerMiddleX + _playerSpeed && left + _player.Width < GameView.Width)
                    _player.SetLeft(left + effectiveSpeed);
            }
            else
            {
                if (_moveLeft && left > 0)
                    _player.SetLeft(left - effectiveSpeed);

                if (_moveRight && left + _player.Width < GameView.Width)
                    _player.SetLeft(left + effectiveSpeed);

                if (_moveUp && top > 0 + (50 * _scale))
                    _player.SetTop(top - effectiveSpeed);

                if (_moveDown && top < GameView.Height - (100 * _scale))
                    _player.SetTop(top + effectiveSpeed);

            }
        }

        #endregion

        #endregion

        #region Score

        private void AddScore(double score)
        {
            if (_isPowerMode)
            {
                switch (_powerUpType)
                {
                    case PowerUpType.DOUBLE_SCORE:
                        score *= 2;
                        break;
                    case PowerUpType.QUAD_SCORE:
                        score *= 4;
                        break;
                    default:
                        break;
                }
            }

            _score += score;
        }

        #endregion

        #region Difficulty

        private void ScaleDifficulty()
        {
            if (_score >= 10 && _score < 20)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 1;
                _playerSpeed = _defaultPlayerSpeed + (1 / 2);
            }
            if (_score >= 20 && _score < 30)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 2;
                _playerSpeed = _defaultPlayerSpeed + (2 / 2);
            }
            if (_score >= 30 && _score < 40)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 3;
                _playerSpeed = _defaultPlayerSpeed + (3 / 2);
            }
            if (_score >= 40 && _score < 50)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 4;
                _playerSpeed = _defaultPlayerSpeed + (4 / 2);
            }
            if (_score >= 50 && _score < 80)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 5;
                _playerSpeed = _defaultPlayerSpeed + (5 / 2);
            }
            if (_score >= 80 && _score < 100)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 6;
                _playerSpeed = _defaultPlayerSpeed + (6 / 2);
            }
            if (_score >= 100 && _score < 130)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 7;
                _playerSpeed = _defaultPlayerSpeed + (7 / 2);
            }
            if (_score >= 130 && _score < 150)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 8;
                _playerSpeed = _defaultPlayerSpeed + (8 / 2);
            }
            if (_score >= 150 && _score < 180)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 9;
                _playerSpeed = _defaultPlayerSpeed + (9 / 2);
            }
            if (_score >= 180 && _score < 200)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 10;
                _playerSpeed = _defaultPlayerSpeed + (10 / 2);
            }
            if (_score >= 200 && _score < 220)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 11;
                _playerSpeed = _defaultPlayerSpeed + (11 / 2);
            }
            if (_score >= 220 && _score < 250)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 12;
                _playerSpeed = _defaultPlayerSpeed + (12 / 2);
            }
            if (_score >= 250 && _score < 300)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 13;
                _playerSpeed = _defaultPlayerSpeed + (13 / 2);
            }
            if (_score >= 300 && _score < 350)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 14;
                _playerSpeed = _defaultPlayerSpeed + (14 / 2);
            }
            if (_score >= 350 && _score < 400)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 15;
                _playerSpeed = _defaultPlayerSpeed + (15 / 2);
            }
            if (_score >= 400 && _score < 500)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 16;
                _playerSpeed = _defaultPlayerSpeed + (16 / 2);
            }
            if (_score >= 500 && _score < 600)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 17;
                _playerSpeed = _defaultPlayerSpeed + (17 / 2);
            }
            if (_score >= 600 && _score < 700)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 18;
                _playerSpeed = _defaultPlayerSpeed + (18 / 2);
            }
            if (_score >= 700 && _score < 800)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 19;
                _playerSpeed = _defaultPlayerSpeed + (19 / 2);
            }
            if (_score >= 800 && _score < 900)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 20;
                _playerSpeed = _defaultPlayerSpeed + (20 / 2);
            }
            if (_score >= 900 && _score < 1000)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 21;
                _playerSpeed = _defaultPlayerSpeed + (21 / 2);
            }
            if (_score >= 1000 && _score < 1200)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 22;
                _playerSpeed = _defaultPlayerSpeed + (22 / 2);
            }
            if (_score >= 1200 && _score < 1400)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 23;
                _playerSpeed = _defaultPlayerSpeed + (23 / 2);
            }
            if (_score >= 1400 && _score < 1600)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 24;
                _playerSpeed = _defaultPlayerSpeed + (24 / 2);
            }
            if (_score >= 1600 && _score < 1800)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 25;
                _playerSpeed = _defaultPlayerSpeed + (25 / 2);
            }
            if (_score >= 1800 && _score < 2000)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 26;
                _playerSpeed = _defaultPlayerSpeed + (26 / 2);
            }
            if (_score >= 2000 && _score < 2200)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 27;
                _playerSpeed = _defaultPlayerSpeed + (27 / 2);
            }
            if (_score >= 2200 && _score < 2400)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 28;
                _playerSpeed = _defaultPlayerSpeed + (28 / 2);
            }
            if (_score >= 2400 && _score < 2600)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 29;
                _playerSpeed = _defaultPlayerSpeed + (29 / 2);
            }
            if (_score >= 2600 && _score < 2800)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 30;
                _playerSpeed = _defaultPlayerSpeed + (30 / 2);
            }
            if (_score >= 2800 && _score < 3000)
            {
                _gameSpeed = _defaultGameSpeed + 1 * 31;
                _playerSpeed = _defaultPlayerSpeed + (31 / 2);
            }
        }

        #endregion

        #region Sound

        private async void StartGameSounds()
        {
            SoundHelper.PlaySound(SoundType.CAR_START);

            await Task.Delay(TimeSpan.FromSeconds(1));

            SoundHelper.PlaySound(SoundType.CAR_ENGINE);

            SoundHelper.RandomizeSound(SoundType.BACKGROUND);
            SoundHelper.PlaySound(SoundType.BACKGROUND);
        }

        private void StopGameSounds()
        {
            SoundHelper.StopSound(SoundType.BACKGROUND);
            SoundHelper.StopSound(SoundType.CAR_ENGINE);
        }

        private void PauseGameSounds()
        {
            SoundHelper.PauseSound(SoundType.BACKGROUND);
            SoundHelper.PauseSound(SoundType.CAR_ENGINE);
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

        #region In Game Message

        private void ShowInGameTextMessage(string resourceKey)
        {
            InGameMessageText.Text = LocalizationHelper.GetLocalizedResource(resourceKey);
            InGameMessagePanel.Visibility = Visibility.Visible;
        }

        private void HideInGameTextMessage()
        {
            InGameMessageText.Text = "";
            InGameMessagePanel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #endregion
    }
}
