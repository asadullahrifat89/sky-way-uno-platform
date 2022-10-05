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
using System.Threading;
using Windows.Foundation;
using Windows.System;

namespace RoadRage
{
    public sealed partial class GamePlayPage : Page
    {
        #region Fields

        private PeriodicTimer GameViewTimer;

        private readonly List<GameObject> GameViewRemovableObjects = new();
        private readonly Random rand = new();

        private Rect playerHitBox;

        private int gameSpeed;
        private readonly int defaultGameSpeed = 4; // TODO: make it 4
        private readonly int playerSpeed = 6;
        private int markNum;
        private int powerUpCounter = 30;
        private int powerModeCounter = 250;
        private readonly int powerModeDelay = 250;

        private int healthCounter = 500;
        private int lives = 3;
        private readonly int maxLives = 3;

        private double score;

        private bool moveLeft;
        private bool moveRight;
        private bool moveUp;
        private bool moveDown;
        private bool isGameOver;
        private bool isPowerMode;
        private bool isGamePaused;

        private bool isRecoveringFromDamage;
        private bool isPointerActivated;
        private readonly TimeSpan frameTime = TimeSpan.FromMilliseconds(18);

        private int accelerationCounter;

        private int damageRecoveryCounter = 100;
        private readonly int damageRecoveryDelay = 500;

        private double windowHeight, windowWidth;
        private double scale;
        private Point pointerPosition;

        private Player player;
        #endregion

        #region Ctor

        public GamePlayPage()
        {
            this.InitializeComponent();

            isGameOver = true;

            windowHeight = Window.Current.Bounds.Height;
            windowWidth = Window.Current.Bounds.Width;

            this.Loaded += GamePlayPage_Loaded;
            this.Unloaded += GamePlayPage_Unloaded;
        }

        #endregion

        #region Events

        #region Page

        private void GamePlayPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += GamePlayPage_SizeChanged;
        }

        private void GamePlayPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= GamePlayPage_SizeChanged;
        }

        private void GamePlayPage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            windowWidth = args.NewSize.Width;
            windowHeight = args.NewSize.Height;

            GameView.Width = windowWidth;
            GameView.Height = windowHeight;

            Console.WriteLine($"WINDOWS SIZE: {windowWidth}x{windowHeight}");

            InitGame();
        }

        #endregion

        #region Input

        private void InputView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isGameOver)
            {
                InputView.Focus(FocusState.Programmatic);
                StartGame();
            }
            else
            {
                isPointerActivated = true;
            }
        }

        private void InputView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPointerActivated)
            {
                PointerPoint point = e.GetCurrentPoint(GameView);
                pointerPosition = point.Position;
            }
        }

        private void InputView_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPointerActivated = false;
            pointerPosition = null;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Left)
            {
                moveLeft = true;
                moveRight = false;
            }
            if (e.Key == VirtualKey.Right)
            {
                moveRight = true;
                moveLeft = false;
            }
            if (e.Key == VirtualKey.Up)
            {
                moveUp = true;
                moveDown = false;
            }
            if (e.Key == VirtualKey.Down)
            {
                moveDown = true;
                moveUp = false;
            }
        }

        private void OnKeyUP(object sender, KeyRoutedEventArgs e)
        {
            // when the player releases the left or right key it will set the designated boolean to false
            if (e.Key == VirtualKey.Left)
            {
                moveLeft = false;
            }
            if (e.Key == VirtualKey.Right)
            {
                moveRight = false;
            }
            if (e.Key == VirtualKey.Up)
            {
                moveUp = false;
            }
            if (e.Key == VirtualKey.Down)
            {
                moveDown = false;
            }

            if (!moveLeft && !moveRight && !moveUp && !moveDown)
                accelerationCounter = 0;

            // in this case we will listen for the enter key aswell but for this to execute we will need the game over boolean to be true
            if (e.Key == VirtualKey.Enter && isGameOver == true)
            {
                StartGame();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Game

        private void InitGame()
        {
            scale = GetGameObjectScale();

            GameView.Children.Clear();

            // add 5 cars
            for (int i = 0; i < 5; i++)
            {
                var car = new Car()
                {
                    Width = Constants.CarWidth * scale,
                    Height = Constants.CarHeight * scale,
                };

                car.SetPosition(rand.Next(100 * (int)scale, (int)GameView.Height) * -1, rand.Next(0, (int)GameView.Width) - (100 * scale));

                GameView.Children.Add(car);
            }

            // add 50 road marks
            for (int i = -25; i < 25; i++)
            {
                var roadMark = new RoadMark()
                {
                    Width = Constants.RoadMarkWidth * scale,
                    Height = Constants.RoadMarkHeight * scale,
                };

                roadMark.SetPosition((int)roadMark.Height * 2 * i, GameView.Width / 2 - roadMark.Width / 2);
                GameView.Children.Add(roadMark);
            }

            // add player
            player = new Player()
            {
                Width = Constants.PlayerWidth * scale,
                Height = Constants.PlayerHeight * scale,
            };

            player.SetPosition(GameView.Height - player.Height - (50 * scale), GameView.Width / 2 - player.Width / 2);

            GameView.Children.Add(player);
        }

        private double GetGameObjectScale()
        {
            return windowWidth switch
            {
                <= 300 => 0.60,
                <= 400 => 0.65,
                <= 500 => 0.70,
                <= 700 => 0.75,
                <= 900 => 0.80,
                <= 1000 => 0.85,
                <= 1400 => 0.90,
                <= 2000 => 0.95,
                _ => 1,
            };
        }

        private void StartGame()
        {
            Console.WriteLine("GAME STARTED");

            lives = maxLives;
            SetLives();

            gameSpeed = defaultGameSpeed;
            RunGame();

            player.Opacity = 1;

            GameView.Background = this.Resources["RoadBackgroundColor"] as SolidColorBrush;

            moveLeft = false;
            moveRight = false;
            moveUp = false;
            moveDown = false;

            isGameOver = false;
            isPowerMode = false;
            powerModeCounter = powerModeDelay;
            isRecoveringFromDamage = false;
            damageRecoveryCounter = damageRecoveryDelay;

            score = 0;
            scoreText.Text = "Score: 0";

            // remove health and power ups, recylce cars
            foreach (GameObject x in GameView.Children.OfType<GameObject>())
            {
                switch ((string)x.Tag)
                {
                    case Constants.CAR_TAG:
                        {
                            RecyleCar(x);
                        }
                        break;
                    case Constants.HEALTH_TAG:
                    case Constants.POWERUP_TAG:
                        {
                            GameViewRemovableObjects.Add(x);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (GameObject y in GameViewRemovableObjects)
            {
                GameView.Children.Remove(y);
            }

            GameViewRemovableObjects.Clear();
        }

        private async void RunGame()
        {
            GameViewTimer = new PeriodicTimer(frameTime);

            while (await GameViewTimer.WaitForNextTickAsync())
            {
                GameViewLoop();
            }
        }

        private void GameViewLoop()
        {
            score += .05; // increase the score by .5 each tick of the timer

            powerUpCounter -= 1;

            scoreText.Text = "Score: " + score.ToString("#");

            playerHitBox = player.GetHitBox(scale);

            if (powerUpCounter < 0)
            {
                SpawnPowerUp();
                powerUpCounter = rand.Next(500, 800);
            }

            if (lives < maxLives)
            {
                healthCounter--;

                if (healthCounter < 0)
                {
                    SpawnHealth();
                    healthCounter = rand.Next(500, 800);
                }
            }

            foreach (GameObject x in GameView.Children.OfType<GameObject>())
            {
                string tag = (string)x.Tag;

                switch (tag)
                {
                    case Constants.CAR_TAG:
                        {
                            UpdateCar(x);
                        }
                        break;
                    case Constants.POWERUP_TAG:
                        {
                            UpdatePowerUp(x);
                        }
                        break;
                    case Constants.HEALTH_TAG:
                        {
                            UpdateHealth(x);
                        }
                        break;
                    case Constants.ROADMARK_TAG:
                        {
                            UpdateRoadMark(x);
                        }
                        break;
                    case Constants.PLAYER_TAG:
                        {
                            if (moveLeft || moveRight || moveUp || moveDown || isPointerActivated)
                            {
                                UpdatePlayer();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (isGameOver)
                return;

            if (isPowerMode)
            {
                PowerUpCoolDown();

                if (powerModeCounter <= 0)
                {
                    PowerDown();
                }
            }

            foreach (GameObject y in GameViewRemovableObjects)
            {
                GameView.Children.Remove(y);
            }

            // as you progress in the game you will score higher and game speed will go up
            ScaleDifficulty();

        }

        private void GameOver()
        {
            //player.SetContent(new Uri("ms-appx:///Assets/Images/player-crashed.png"));

            StopGame();

            scoreText.Text += " Press Enter to replay";
            isGameOver = true;
        }

        private void StopGame()
        {
            GameViewTimer.Dispose();
        }

        #endregion

        #region Car

        private void UpdateCar(GameObject vehicle)
        {
            // move down vehicle
            vehicle.SetTop(vehicle.GetTop() + vehicle.Speed);

            // if vechicle goes out of bounds
            if (vehicle.GetTop() > GameView.Height)
            {
                RecyleCar(vehicle);
            }

            if (isRecoveringFromDamage)
            {
                player.Opacity = 0.66;
                damageRecoveryCounter--;

                if (damageRecoveryCounter <= 0)
                {
                    player.Opacity = 1;
                    isRecoveringFromDamage = false;
                }
            }
            else
            {
                // if vehicle collides with player
                if (playerHitBox.IntersectsWith(vehicle.GetHitBox(scale)))
                {
                    if (!isPowerMode)
                    {
                        lives--;
                        damageRecoveryCounter = damageRecoveryDelay;
                        isRecoveringFromDamage = true;
                        SetLives();

                        if (lives == 0)
                            GameOver();
                    }
                }
            }

            if (isGameOver)
                return;

            //TODO: this is expensive
            // if vechicle will collide with another vehicle
            if (GameView.Children.OfType<GameObject>()
                .Where(x => (string)x.Tag is Constants.CAR_TAG)
                .LastOrDefault(v => v.GetDistantHitBox(scale)
                .IntersectsWith(vehicle.GetDistantHitBox(scale))) is GameObject collidingVehicle)
            {
                // slower vehicles will slow down faster vehicles
                if (collidingVehicle.Speed > vehicle.Speed)
                {
                    vehicle.Speed = collidingVehicle.Speed;
                }
                else
                {
                    collidingVehicle.Speed = vehicle.Speed;
                }
            }
        }

        private void RecyleCar(GameObject car)
        {
            markNum = rand.Next(0, Constants.CAR_TEMPLATES.Length);

            car.SetContent(Constants.CAR_TEMPLATES[markNum]);
            car.SetSize(Constants.CarWidth * scale, Constants.CarHeight * scale);
            car.Speed = gameSpeed - rand.Next(0, 6);

            // set a random top and left position for the traffic car
            car.SetPosition(rand.Next(100, (int)GameView.Height) * -1, rand.Next(0, (int)GameView.Width - 50));
        }

        #endregion

        #region Player

        private void UpdatePlayer()
        {
            double effectiveSpeed = accelerationCounter >= playerSpeed ? playerSpeed : accelerationCounter / 1.3;

            // increase acceleration and stop when player speed is reached
            if (accelerationCounter <= playerSpeed)
                accelerationCounter++;

            //Console.WriteLine("ACC:" + _accelerationCounter);            

            double left = player.GetLeft();
            double top = player.GetTop();

            double playerMiddleX = left + player.Width / 2;
            double playerMiddleY = top + player.Height / 2;

            if (isPointerActivated)
            {
                // move up
                if (pointerPosition.Y < playerMiddleY - playerSpeed)
                {
                    player.SetTop(top - effectiveSpeed);
                }
                // move left
                if (pointerPosition.X < playerMiddleX - playerSpeed && left > 0)
                {
                    player.SetLeft(left - effectiveSpeed);
                }

                // move down
                if (pointerPosition.Y > playerMiddleY + playerSpeed)
                {
                    player.SetTop(top + effectiveSpeed);
                }
                // move right
                if (pointerPosition.X > playerMiddleX + playerSpeed && left + player.Width < GameView.Width)
                {
                    player.SetLeft(left + effectiveSpeed);
                }
            }
            else
            {
                if (moveLeft && left > 0)
                {
                    player.SetLeft(left - effectiveSpeed);
                }
                if (moveRight && left + player.Width < GameView.Width)
                {
                    player.SetLeft(left + effectiveSpeed);
                }
                if (moveUp && top > 0 + (50 * scale))
                {
                    player.SetTop(top - effectiveSpeed);
                }
                if (moveDown && top < GameView.Height - (100 * scale))
                {
                    player.SetTop(top + effectiveSpeed);
                }
            }
        }

        #endregion

        #region Power Up

        private void UpdatePowerUp(GameObject powerUp)
        {
            powerUp.SetTop(powerUp.GetTop() + 5);

            // if player gets a power up
            if (playerHitBox.IntersectsWith(powerUp.GetHitBox(scale)))
            {
                GameViewRemovableObjects.Add(powerUp);

                TriggerPowerUp();
            }

            if (powerUp.GetTop() > GameView.Height)
            {
                GameViewRemovableObjects.Add(powerUp);
            }
        }

        private void TriggerPowerUp()
        {
            powerUpText.Visibility = Visibility.Visible;
            isPowerMode = true;
            powerModeCounter = powerModeDelay;
        }

        private void PowerUpCoolDown()
        {
            powerModeCounter -= 1;
            GameView.Background = new SolidColorBrush(Colors.Goldenrod);
            player.Opacity = 0.77;

            double remainingPow = (double)powerModeCounter / (double)powerModeDelay * 4;

            powerUpText.Text = "";
            for (int i = 0; i < remainingPow; i++)
            {
                powerUpText.Text += "⚡";
            }
        }

        private void PowerDown()
        {
            isPowerMode = false;
            player.Opacity = 1;
            powerUpText.Visibility = Visibility.Collapsed;

            GameView.Background = this.Resources["RoadBackgroundColor"] as SolidColorBrush;
        }

        private void SpawnPowerUp()
        {
            PowerUp powerUp = new()
            {
                Height = 50 * scale,
                Width = 50 * scale,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { Angle = Convert.ToDouble(this.Resources["FoliageViewRotationAngle"]) },
            };

            powerUp.SetPosition(rand.Next(100, (int)GameView.Height) * -1, rand.Next(0, (int)(GameView.Width - 55)));

            GameView.Children.Add(powerUp);
        }

        #endregion

        #region Health

        private void SetLives()
        {
            livesText.Text = "";
            for (int i = 0; i < lives; i++)
            {
                livesText.Text += "❤️";
            }
        }

        private void SpawnHealth()
        {
            Health health = new()
            {
                Height = 80 * scale,
                Width = 80 * scale,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { Angle = Convert.ToDouble(this.Resources["FoliageViewRotationAngle"]) },
            };

            health.SetPosition(rand.Next(100, (int)GameView.Height) * -1, rand.Next(0, (int)(GameView.Width - 55)));
            GameView.Children.Add(health);
        }

        private void UpdateHealth(GameObject health)
        {
            health.SetTop(health.GetTop() + 5);

            // if player gets a health
            if (playerHitBox.IntersectsWith(health.GetHitBox(scale)))
            {
                GameViewRemovableObjects.Add(health);

                lives++;
                SetLives();
            }

            if (health.GetTop() > GameView.Height)
            {
                GameViewRemovableObjects.Add(health);
            }
        }

        #endregion

        #region Road Marks

        private void UpdateRoadMark(GameObject roadMark)
        {
            roadMark.SetTop(roadMark.GetTop() + gameSpeed);

            if (roadMark.GetTop() > GameView.Height)
            {
                RecyleRoadMark(roadMark);
            }
        }

        private void RecyleRoadMark(GameObject roadMark)
        {
            roadMark.SetSize(Constants.RoadMarkWidth * scale, Constants.RoadMarkHeight * scale);
            roadMark.SetTop((int)roadMark.Height * 2 * -25);
        }

        #endregion

        #region Game Difficulty

        private void ScaleDifficulty()
        {
            if (score >= 10 && score < 20)
            {
                gameSpeed = defaultGameSpeed + 2;
            }

            if (score >= 20 && score < 30)
            {
                gameSpeed = defaultGameSpeed + 4;
            }
            if (score >= 30 && score < 40)
            {
                gameSpeed = defaultGameSpeed + 6;
            }
            if (score >= 40 && score < 50)
            {
                gameSpeed = defaultGameSpeed + 8;
            }
            if (score >= 50 && score < 80)
            {
                gameSpeed = defaultGameSpeed + 10;
            }
            if (score >= 80 && score < 100)
            {
                gameSpeed = defaultGameSpeed + 12;
            }
            if (score >= 100 && score < 130)
            {
                gameSpeed = defaultGameSpeed + 14;
            }
            if (score >= 130 && score < 150)
            {
                gameSpeed = defaultGameSpeed + 16;
            }
            if (score >= 150 && score < 180)
            {
                gameSpeed = defaultGameSpeed + 18;
            }
            if (score >= 180 && score < 200)
            {
                gameSpeed = defaultGameSpeed + 20;
            }
        }

        #endregion

        #endregion
    }
}
