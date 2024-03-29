﻿using System;
using System.Collections.Generic;

namespace SkyWayGame
{
    public static class Constants
    {
        public const string GAME_ID = "sky-way";
        public const string COMPANY_ID = "selise";

        #region Measurements

        public const double DEFAULT_FRAME_TIME = 18;

        public const double CAR_WIDTH = 50 * 1.5;
        public const double CAR_HEIGHT = 110 * 1.5;

        public const double PLAYER_WIDTH = 55 * 1.5;
        public const double PLAYER_HEIGHT = 115 * 1.5;

        public const double POWERUP_WIDTH = 80;
        public const double POWERUP_HEIGHT = 80;

        public const double COLLECTIBLE_WIDTH = 60;
        public const double COLLECTIBLE_HEIGHT = 60;

        public const double HEALTH_WIDTH = 80;
        public const double HEALTH_HEIGHT = 80;

        public const double CLOUD_WIDTH = 100;
        public const double CLOUD_HEIGHT = 100;

        public const double ISLAND_WIDTH = 600;
        public const double ISLAND_HEIGHT = 600;

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
            new KeyValuePair<ElementType, Uri>(ElementType.POWERUP, new Uri("ms-appx:///Assets/Images/powerup1.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.POWERUP, new Uri("ms-appx:///Assets/Images/powerup2.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.POWERUP, new Uri("ms-appx:///Assets/Images/powerup3.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.POWERUP, new Uri("ms-appx:///Assets/Images/powerup4.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.HEALTH, new Uri("ms-appx:///Assets/Images/health.png")),
            new KeyValuePair<ElementType, Uri>(ElementType.COLLECTIBLE, new Uri("ms-appx:///Assets/Images/collectible.png")),
        };

        #endregion

        #region Sounds

        public static KeyValuePair<SoundType, string>[] SOUND_TEMPLATES = new KeyValuePair<SoundType, string>[]
        {
            new KeyValuePair<SoundType, string>(SoundType.MENU_SELECT, "Assets/Sounds/menu-select.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.INTRO, "Assets/Sounds/intro1.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.INTRO, "Assets/Sounds/intro2.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.BACKGROUND, "Assets/Sounds/background1.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.BACKGROUND, "Assets/Sounds/background2.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.BACKGROUND, "Assets/Sounds/background3.mp3"),


            new KeyValuePair<SoundType, string>(SoundType.GAME_OVER, "Assets/Sounds/game-over.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.POWER_UP, "Assets/Sounds/power-up.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.POWER_DOWN, "Assets/Sounds/power-down.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.HEALTH_GAIN, "Assets/Sounds/health-gain.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.HEALTH_LOSS, "Assets/Sounds/health-loss.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.COLLECTIBLE_COLLECTED, "Assets/Sounds/coin-pickup.mp3"),

            new KeyValuePair<SoundType, string>(SoundType.CAR_START, "Assets/Sounds/car-start.mp3"),
            new KeyValuePair<SoundType, string>(SoundType.CAR_ENGINE, "Assets/Sounds/car-engine.mp3"),
        };

        #endregion

        #region Web Api Endpoints

        public const string Action_Ping = "/api/Query/Ping";

        public const string Action_Authenticate = "/api/Command/Authenticate";
        public const string Action_SignUp = "/api/Command/SignUp";
        public const string Action_SubmitGameScore = "/api/Command/SubmitGameScore";
        public const string Action_GenerateSession = "/api/Command/GenerateSession";
        public const string Action_ValidateToken = "/api/Command/ValidateToken";

        public const string Action_GetGameProfile = "/api/Query/GetGameProfile";
        public const string Action_GetGameProfiles = "/api/Query/GetGameProfiles";
        public const string Action_GetGameScoresOfTheDay = "/api/Query/GetGameScoresOfTheDay";
        public const string Action_CheckIdentityAvailability = "/api/Query/CheckIdentityAvailability";
        public const string Action_GetSeason = "/api/Query/GetSeason";
        public const string Action_GetGamePrizeOfTheDay = "/api/Query/GetGamePrizeOfTheDay";
        public const string Action_GetCompany = "/api/Query/GetCompany";

        #endregion

        #region Session Keys

        public const string CACHE_REFRESH_TOKEN_KEY = "RT";
        public const string CACHE_LANGUAGE_KEY = "Lang";

        #endregion

        #region Cookie Keys

        public const string COOKIE_KEY = "Cookie";
        public const string COOKIE_ACCEPTED_KEY = "Accepted";

        #endregion
    }
}
