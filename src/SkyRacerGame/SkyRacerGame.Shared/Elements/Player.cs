using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyRacerGame
{
    public class Player : GameObject
    {
        public Player()
        {
            Tag = ElementType.PLAYER;
            SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is ElementType.PLAYER).Value);
        }
    }
}
