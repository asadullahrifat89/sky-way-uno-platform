using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public static class Constants
    {
        #region Tags

        //public const string PLAYER = "player";
        //public const string PLAYER_POWER_MODE = "player-power-mode";

        //public const string CAR = "car";

        //public const string POWERUP = "powerup";
        //public const string HEALTH = "health";

        //public const string ROADMARK = "roadmark";
        //public const string ROADDIVIDER = "roaddivider";

        //public const string CLOUD = "cloud";
        //public const string ISLAND = "island";

        //public const string COLLECTIBLE = "collectible";

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

        public static KeyValuePair<ElementType, Uri>[] ELEMENT_TEMPLATES = new KeyValuePair<ElementType, Uri>[]
        {
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car1.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car2.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car3.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car4.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car5.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car6.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car7.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car8.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car9.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car10.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CAR, new Uri("ms-appx:///Assets/Images/car11.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CLOUD, new Uri("ms-appx:///Assets/Images/cloud1.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.CLOUD, new Uri("ms-appx:///Assets/Images/cloud2.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.ISLAND, new Uri("ms-appx:///Assets/Images/island1.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.ISLAND, new Uri("ms-appx:///Assets/Images/island2.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.ISLAND, new Uri("ms-appx:///Assets/Images/island3.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.PLAYER, new Uri("ms-appx:///Assets/Images/player.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.PLAYER_POWER_MODE, new Uri("ms-appx:///Assets/Images/player-power-mode.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.POWERUP, new Uri("ms-appx:///Assets/Images/powerup.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.HEALTH, new Uri("ms-appx:///Assets/Images/health.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.COLLECTIBLE, new Uri("ms-appx:///Assets/Images/collectible.png")),
        };

        #endregion

        #region Sounds

        public static KeyValuePair<SoundType, string>[] SOUND_TEMPLATES = new KeyValuePair<SoundType, string>[]
        {
            new KeyValuePair<SoundType, string>(SoundType.BACKGROUND_MUSIC, "Assets/Sounds/Intro/intro1.mp3")
        };

        #endregion
    }

    public enum ElementType
    {
        NONE,
        PLAYER,
        PLAYER_POWER_MODE,
        CAR,
        POWERUP,
        HEALTH,
        ROADMARK,
        ROADDIVIDER,
        CLOUD,
        ISLAND,
        COLLECTIBLE,
    }

    public enum SoundType
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
