namespace SkyWay
{
    public class PowerUp : GameObject
    {
        public PowerUp()
        {
            Tag = Constants.POWERUP_TAG;
            SetContent(Constants.POWERUP_TEMPLATE);
        }
    }
}

