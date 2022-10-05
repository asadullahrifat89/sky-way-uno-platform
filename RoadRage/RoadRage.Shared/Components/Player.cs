using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadRage
{
    public class Player : GameObject
    {
        public Player()
        {
            Tag = Constants.PLAYER_TAG;
            SetContent(Constants.PLAYER_TEMPLATE);
        }
    }
}
