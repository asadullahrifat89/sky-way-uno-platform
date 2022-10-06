using System.Linq;

namespace SkyWay
{
    public class PowerUp : GameObject
    {
        public PowerUp()
        {
            Tag = Constants.POWERUP_TAG;
            SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is Constants.POWERUP_TAG).Value);
        }
    }
}

