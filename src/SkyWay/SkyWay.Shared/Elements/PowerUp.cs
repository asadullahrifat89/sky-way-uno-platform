using System.Linq;

namespace SkyWay
{
    public class PowerUp : GameObject
    {
        public PowerUp()
        {
            Tag = ElementType.POWERUP;
            SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is ElementType.POWERUP).Value);
        }
    }
}

