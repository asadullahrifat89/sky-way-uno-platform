﻿using System.Linq;

namespace SkyWayGame
{
    public class Health : GameObject
    {
        public Health()
        {
            Tag = ElementType.HEALTH;
            SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is ElementType.HEALTH).Value);
        }
    }
}

