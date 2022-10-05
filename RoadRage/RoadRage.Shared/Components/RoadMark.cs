using Microsoft.UI;
using Microsoft.UI.Xaml;
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

            Background = new SolidColorBrush(Colors.White);
            BorderBrush = new SolidColorBrush(Colors.Wheat);
            BorderThickness = new Microsoft.UI.Xaml.Thickness(5);
            CornerRadius = new Microsoft.UI.Xaml.CornerRadius(5);
        }
    }

    public class RoadDivider : GameObject
    {
        public RoadDivider()
        {
            Tag = Constants.ROADDIVIDER_TAG;

            Background = Application.Current.Resources["RoadDividerColor"] as SolidColorBrush;
            BorderBrush = new SolidColorBrush(Colors.DarkGoldenrod);
        }
    }
}

