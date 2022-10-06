using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public static class Constants
    {
        #region Tags

        public const string PLAYER_TAG = "player";
        public const string PLAYER_POWER_MODE_TAG = "player-power-mode";

        public const string CAR_TAG = "car";

        public const string POWERUP_TAG = "powerup";
        public const string HEALTH_TAG = "health";

        public const string ROADMARK_TAG = "roadmark";
        public const string ROADDIVIDER_TAG = "roaddivider";

        public const string CLOUD_TAG = "cloud";
        public const string ISLAND_TAG = "island";

        public const string COLLECTIBLE_TAG = "collectible";

        #endregion

        #region Measurements

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

        public const double ISLAND_WIDTH = 600;
        public const double ISLAND_HEIGHT = 600;

        public const double ROADDIVIDER_WIDTH = 30;

        #endregion

        #region Images

        public static KeyValuePair<string, Uri>[] ELEMENT_TEMPLATES = new KeyValuePair<string, Uri>[]
        {
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car1.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car2.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car3.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car4.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car5.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car6.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car7.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car8.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car9.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car10.png")),
            new KeyValuePair<string, Uri>(CAR_TAG, new Uri("ms-appx:///Assets/Images/car11.png")),

            new KeyValuePair<string, Uri>(CLOUD_TAG, new Uri("ms-appx:///Assets/Images/cloud1.png")),
            new KeyValuePair<string, Uri>(CLOUD_TAG, new Uri("ms-appx:///Assets/Images/cloud2.png")),

            new KeyValuePair<string, Uri>(ISLAND_TAG, new Uri("ms-appx:///Assets/Images/island1.png")),
            new KeyValuePair<string, Uri>(ISLAND_TAG, new Uri("ms-appx:///Assets/Images/island2.png")),
            new KeyValuePair<string, Uri>(ISLAND_TAG, new Uri("ms-appx:///Assets/Images/island3.png")),

            new KeyValuePair<string, Uri>(PLAYER_TAG, new Uri("ms-appx:///Assets/Images/player.png")),
            new KeyValuePair<string, Uri>(PLAYER_POWER_MODE_TAG, new Uri("ms-appx:///Assets/Images/player-power-mode.png")),

            new KeyValuePair<string, Uri>(POWERUP_TAG, new Uri("ms-appx:///Assets/Images/powerup.png")),
            new KeyValuePair<string, Uri>(HEALTH_TAG, new Uri("ms-appx:///Assets/Images/health.png")),
            new KeyValuePair<string, Uri>(COLLECTIBLE_TAG, new Uri("ms-appx:///Assets/Images/collectible.png")),

            //new (new Uri("ms-appx:///Assets/Images/car1.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car2.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car3.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car4.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car5.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car6.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car7.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car8.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car9.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car10.png"), CAR_TAG),
            //new (new Uri("ms-appx:///Assets/Images/car11.png"), CAR_TAG),

            //new ( new Uri("ms-appx:///Assets/Images/cloud1.png"), CLOUD_TAG),
            //new ( new Uri("ms-appx:///Assets/Images/cloud2.png"), CLOUD_TAG),

            //new ( new Uri("ms-appx:///Assets/Images/island1.png"), ISLAND_TAG),
            //new ( new Uri("ms-appx:///Assets/Images/island2.png"), ISLAND_TAG),
            //new ( new Uri("ms-appx:///Assets/Images/island3.png"), ISLAND_TAG),

            //new ( new Uri("ms-appx:///Assets/Images/player.png"), PLAYER_TAG),
            //new ( new Uri("ms-appx:///Assets/Images/player-power-mode.png"), PLAYER_POWER_MODE_TAG),

            //new ( new Uri("ms-appx:///Assets/Images/powerup.png"), POWERUP_TAG),
            //new ( new Uri("ms-appx:///Assets/Images/health.png"), HEALTH_TAG),
            //new ( new Uri("ms-appx:///Assets/Images/collectible.png"), COLLECTIBLE_TAG),
        };

        //public static Uri[] CAR_TEMPLATES = new Uri[]
        //{
        //    new Uri("ms-appx:///Assets/Images/car1.png"),
        //    new Uri("ms-appx:///Assets/Images/car2.png"),
        //    new Uri("ms-appx:///Assets/Images/car3.png"),
        //    new Uri("ms-appx:///Assets/Images/car4.png"),
        //    new Uri("ms-appx:///Assets/Images/car5.png"),
        //    new Uri("ms-appx:///Assets/Images/car6.png"),
        //    new Uri("ms-appx:///Assets/Images/car7.png"),
        //    new Uri("ms-appx:///Assets/Images/car8.png"),
        //    new Uri("ms-appx:///Assets/Images/car9.png"),
        //    new Uri("ms-appx:///Assets/Images/car10.png"),
        //    new Uri("ms-appx:///Assets/Images/car11.png"),
        //};

        //public static Uri[] CLOUD_TEMPLATES = new Uri[]
        //{
        //    new Uri("ms-appx:///Assets/Images/cloud1.png"),
        //    new Uri("ms-appx:///Assets/Images/cloud2.png"),
        //};

        //public static Uri[] ISLAND_TEMPLATES = new Uri[]
        //{
        //    new Uri("ms-appx:///Assets/Images/island1.png"),
        //    new Uri("ms-appx:///Assets/Images/island2.png"),
        //    new Uri("ms-appx:///Assets/Images/island3.png"),
        //};

        //public static Uri PLAYER_TEMPLATE = new("ms-appx:///Assets/Images/player.png");
        //public static Uri PLAYER_POWER_MODE_TEMPLATE = new("ms-appx:///Assets/Images/player-power-mode.png");
        //public static Uri POWERUP_TEMPLATE = new("ms-appx:///Assets/Images/powerup.png");
        //public static Uri HEALTH_TEMPLATE = new("ms-appx:///Assets/Images/health.png");
        //public static Uri COLLECTIBLE_TEMPLATE = new("ms-appx:///Assets/Images/collectible.png");

        #endregion

        #region Sounds

        public static (string Url, SoundTypes SoundType)[] SOUND_TEMPLATES = new (string Url, SoundTypes SoundType)[]
        {
            new (),
        };

        #endregion
    }

    public enum SoundTypes
    {
        GAME_INTRO,
        MENU_SELECT,
        BACKGROUND_MUSIC,
        POWER_UP,
        POWER_DOWN,
        HEALTH_GAIN,
        HEALTH_LOSS,
        COLLECTIBLE_COLLECTED,
        CAR_START,
        GAME_OVER,
    }
}
