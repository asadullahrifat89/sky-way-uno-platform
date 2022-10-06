using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public static class Constants
    {
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
            new KeyValuePair<SoundType, string>(SoundType.BACKGROUND, "Assets/Sounds/background1.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.BACKGROUND, "Assets/Sounds/background2.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.BACKGROUND, "Assets/Sounds/background3.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.INTRO, "Assets/Sounds/intro1.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.INTRO, "Assets/Sounds/intro2.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.GAME_START, "Assets/Sounds/spaceship-start.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.GAME_OVER, "Assets/Sounds/car-crash.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.COLLECTIBLE_COLLECTED, "Assets/Sounds/coin-pickup.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.POWER_UP, "Assets/Sounds/powerup.mp3"),
        };

        #endregion
    }
}
