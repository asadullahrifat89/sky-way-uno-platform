﻿using System.Linq;

namespace SkyWayGame
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
