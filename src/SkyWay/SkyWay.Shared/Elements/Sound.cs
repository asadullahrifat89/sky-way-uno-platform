using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public class Sound
    {
        private readonly AudioPlayer _audioPlayer;

        public Sound(SoundTypes soundType)
        {
            switch (soundType)
            {
                case SoundTypes.GAME_INTRO:
                    break;
                case SoundTypes.MENU_SELECT:
                    break;
                case SoundTypes.BACKGROUND_MUSIC:
                    break;
                case SoundTypes.POWER_UP:
                    break;
                case SoundTypes.POWER_DOWN:
                    break;
                case SoundTypes.HEALTH_GAIN:
                    break;
                case SoundTypes.HEALTH_LOSS:
                    break;
                case SoundTypes.COLLECTIBLE_COLLECTED:
                    break;
                case SoundTypes.CAR_START:
                    break;
                case SoundTypes.GAME_OVER:
                    break;
                default:
                    break;
            }
        }
    }
}
