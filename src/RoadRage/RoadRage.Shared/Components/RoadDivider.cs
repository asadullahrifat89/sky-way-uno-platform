using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace RoadRage
{
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

