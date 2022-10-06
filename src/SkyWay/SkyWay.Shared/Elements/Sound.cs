using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyWay
{
    public class Sound
    {
        private readonly AudioPlayer _audioPlayer;
        private readonly Random random;

        public Sound(SoundType soundType)
        {
            random = new Random();
            var baseUrl = App.GetBaseUrl();

            switch (soundType)
            {
                case SoundType.INTRO:
                    {
                        var tracks = Constants.SOUND_TEMPLATES.Where(x => x.Key == SoundType.INTRO).ToArray();
                        var trackNum = random.Next(0, tracks.Length);
                        var track = tracks[trackNum];

                        var source = string.Concat(baseUrl, "/", track);

                        _audioPlayer = new AudioPlayer(
                            source: source, 
                            volume: 0.5, 
                            loop: true);
                    }
                    break;
                case SoundType.MENU_SELECT:
                    {

                    }
                    break;
                case SoundType.BACKGROUND:
                    {

                    }
                    break;
                case SoundType.POWER_UP:
                    {

                    }
                    break;
                case SoundType.POWER_DOWN:
                    {

                    }
                    break;
                case SoundType.HEALTH_GAIN:
                    {

                    }
                    break;
                case SoundType.HEALTH_LOSS:
                    {

                    }
                    break;
                case SoundType.COLLECTIBLE_COLLECTED:
                    {

                    }
                    break;
                case SoundType.GAME_START:
                    {

                    }
                    break;
                case SoundType.GAME_OVER:
                    {

                    }
                    break;
                default:
                    break;
            }
        }

        public void Play()
        {
            _audioPlayer.Play();
        }

        public void Stop()
        {
            _audioPlayer.Stop();
        }

        public void Pause()
        {
            _audioPlayer.Pause();
        }

        public void Resume()
        {
            _audioPlayer.Resume();
        }
    }

    public enum SoundType
    {
        MENU_SELECT,
        INTRO,
        BACKGROUND,
        POWER_UP,
        POWER_DOWN,
        HEALTH_GAIN,
        HEALTH_LOSS,
        COLLECTIBLE_COLLECTED,
        GAME_START,
        GAME_OVER,
    }
}
