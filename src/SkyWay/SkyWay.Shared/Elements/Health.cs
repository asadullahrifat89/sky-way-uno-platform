using System.Linq;

namespace SkyWay
{
    public class Health : GameObject
    {
        public Health()
        {
            Tag = Constants.HEALTH_TAG;
            SetContent(Constants.ELEMENT_TEMPLATES.FirstOrDefault(x => x.Key is Constants.HEALTH_TAG).Value);
        }
    }
}

