namespace SkyWay
{
    public class Health : GameObject
    {
        public Health()
        {
            Tag = Constants.HEALTH_TAG;
            SetContent(Constants.HEALTH_TEMPLATE);
        }
    }
}

