using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace RoadRage
{
    public class RoadMark : GameObject
    {
        public RoadMark()
        {
            Tag = Constants.ROADMARK_TAG;            
            SetContent(new Uri("ms-appx:///Assets/Images/road-mark2.png"));
        }
    }
}

