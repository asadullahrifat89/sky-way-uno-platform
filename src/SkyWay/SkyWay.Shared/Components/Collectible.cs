namespace SkyWay
{
    public class Collectible : GameObject
    {
        public Collectible()
        {
            Tag = Constants.COLLECTIBLE_TAG;
            SetContent(Constants.COLLECTIBLE_TEMPLATE);
        }
    }
}

