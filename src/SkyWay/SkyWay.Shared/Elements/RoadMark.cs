using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SkyWay
{
    public class RoadMark : GameObject
    {
        public RoadMark()
        {
            Tag = ElementType.ROADMARK;

            Background = new SolidColorBrush(Colors.White);
            BorderBrush = new SolidColorBrush(Colors.Wheat);
            BorderThickness = new Microsoft.UI.Xaml.Thickness(5);
            CornerRadius = new Microsoft.UI.Xaml.CornerRadius(5);
        }
    }
}

