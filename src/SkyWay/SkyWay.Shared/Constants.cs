using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public static class Constants
    {
        public const string PLAYER_TAG = "player";

        public const string CAR_TAG = "car";

        public const string POWERUP_TAG = "powerup";
        public const string HEALTH_TAG = "health";

        public const string ROADMARK_TAG = "roadmark";
        public const string ROADDIVIDER_TAG = "roaddivider";

        public const string CLOUD_TAG = "cloud";

        public const string COLLECTIBLE_TAG = "collectible";

        public const double CAR_WIDTH = 60 * 1.5;
        public const double CAR_HEIGHT = 120 * 1.5;

        public const double PLAYER_WIDTH = 60 * 1.5;
        public const double PLAYER_HEIGHT = 120 * 1.5;

        public const double POWERUP_WIDTH = 80;
        public const double POWERUP_HEIGHT = 80;

        public const double COLLECTIBLE_WIDTH = 60;
        public const double COLLECTIBLE_HEIGHT = 60;

        public const double HEALTH_WIDTH = 80;
        public const double HEALTH_HEIGHT = 80;

        public const double ROADMARK_WIDTH = 30;
        public const double ROADMARK_HEIGHT = 80;

        public const double CLOUD_WIDTH = 100;
        public const double CLOUD_HEIGHT = 100;

        public const double ROADDIVIDER_WIDTH = 30;

        public static Uri[] CAR_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/car1.png"),
            new Uri("ms-appx:///Assets/Images/car2.png"),
            new Uri("ms-appx:///Assets/Images/car3.png"),
            new Uri("ms-appx:///Assets/Images/car4.png"),
            new Uri("ms-appx:///Assets/Images/car5.png"),
        };

        public static Uri[] CLOUD_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/cloud1.png"),
            new Uri("ms-appx:///Assets/Images/cloud2.png"),
        };

        public static Uri PLAYER_TEMPLATE = new Uri("ms-appx:///Assets/Images/player.png");
        public static Uri PLAYER_POWER_MODE_TEMPLATE = new Uri("ms-appx:///Assets/Images/player-power-mode.png");
        public static Uri POWERUP_TEMPLATE = new Uri("ms-appx:///Assets/Images/powerup.png");

        public static Uri HEALTH_TEMPLATE = new Uri("ms-appx:///Assets/Images/health.png");

        public static Uri COLLECTIBLE_TEMPLATE = new Uri("ms-appx:///Assets/Images/collectible.png");
    }
}
