using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public class Sound
    {
        private readonly AudioPlayer _audioPlayer;

        public Sound(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.GAME_INTRO:
                    break;
                case SoundType.MENU_SELECT:
                    break;
                case SoundType.BACKGROUND_MUSIC:
                    break;
                case SoundType.POWER_UP:
                    break;
                case SoundType.POWER_DOWN:
                    break;
                case SoundType.HEALTH_GAIN:
                    break;
                case SoundType.HEALTH_LOSS:
                    break;
                case SoundType.COLLECTIBLE_COLLECTED:
                    break;
                case SoundType.CAR_START:
                    break;
                case SoundType.GAME_OVER:
                    break;
                default:
                    break;
            }
        }
    }
}
