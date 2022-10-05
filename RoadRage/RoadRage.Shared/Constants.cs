using System;
using System.Collections.Generic;
using System.Text;

namespace RoadRage
{
    public static class Constants
    {
        public const string PLAYER_TAG = "player";

        public const string CAR_TAG = "car";

        public const string POWERUP_TAG = "powerup";
        public const string HEALTH_TAG = "health";

        public const string ROADMARK_TAG = "roadmark";
        public const string ROADDIVIDER_TAG = "roaddivider";

        public const double CarWidth = 60 * 1.5;
        public const double CarHeight = 120 * 1.5;

        public const double PlayerWidth = 70 * 1.5;
        public const double PlayerHeight = 130 * 1.5;

        public const double PowerUpWidth = 40 * 1.5;
        public const double PowerUpHeight = 40 * 1.5;

        public const double RoadMarkWidth = 30;
        public const double RoadMarkHeight = 80;

        public const double RoadDividerWidth = 30;

        public static Uri[] CAR_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/car1.png"),
            new Uri("ms-appx:///Assets/Images/car2.png"),
            new Uri("ms-appx:///Assets/Images/car3.png"),
            new Uri("ms-appx:///Assets/Images/car4.png"),
            new Uri("ms-appx:///Assets/Images/car5.png"),
        };

        public static Uri PLAYER_TEMPLATE = new Uri("ms-appx:///Assets/Images/player.png");
        public static Uri PLAYER_POWER_MODE_TEMPLATE = new Uri("ms-appx:///Assets/Images/player-power-mode.png");
        public static Uri POWERUP_TEMPLATE = new Uri("ms-appx:///Assets/Images/powerup.png");

        public static Uri HEALTH_TEMPLATE = new Uri("ms-appx:///Assets/Images/health.png");
    }
}
