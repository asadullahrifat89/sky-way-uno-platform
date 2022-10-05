using Microsoft.UI.Xaml.Media;
using System;

namespace SkyWay
{
    public class Cloud : GameObject
    {
        public Cloud()
        {
            Tag = Constants.CLOUD_TAG;

            var rand = new Random();

            var carNum = rand.Next(0, Constants.CLOUD_TEMPLATES.Length);
            SetContent(Constants.CLOUD_TEMPLATES[carNum]);

            var scaleFactor = rand.Next(1, 4);
            var scaleReverseFactor = rand.Next(-1, 2);

            RenderTransform = new CompositeTransform()
            {
                ScaleX = scaleFactor * scaleReverseFactor,
                ScaleY = scaleFactor,
            };
        }
    }
}

