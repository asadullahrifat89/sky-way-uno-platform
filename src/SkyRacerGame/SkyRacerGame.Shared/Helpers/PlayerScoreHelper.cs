using System;
using System.Collections.Generic;
using System.Text;

namespace SkyRacerGame
{
    public static class PlayerScoreHelper
    {
        public static SkyRacerGameScore PlayerScore { get; set; }

        public static bool GameScoreSubmissionPending { get; set; }
    }
}
