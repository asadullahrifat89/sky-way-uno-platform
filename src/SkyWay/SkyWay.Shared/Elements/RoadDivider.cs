using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace SkyWay
{
    public class RoadDivider : GameObject
    {
        public RoadDivider()
        {
            Tag = ElementType.ROADDIVIDER;            

            Background = Application.Current.Resources["RoadDividerColor"] as SolidColorBrush;
            BorderBrush = new SolidColorBrush(Colors.DarkGoldenrod);
        }
    }
}

