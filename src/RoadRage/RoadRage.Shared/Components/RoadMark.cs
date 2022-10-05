using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

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
}

