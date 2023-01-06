using System.Linq;

namespace SkyWayGame
{
    public class Collectible : GameObject
    {
        public Collectible()
        {
            Tag = ElementType.COLLECTIBLE;
            SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is ElementType.COLLECTIBLE).Value);
        }
    }
}

