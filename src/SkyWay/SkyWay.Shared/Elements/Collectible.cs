using System.Linq;

namespace SkyWay
{
    public class Collectible : GameObject
    {
        public Collectible()
        {
            Tag = Constants.COLLECTIBLE_TAG;
            SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is Constants.COLLECTIBLE_TAG).Value);
        }
    }
}

