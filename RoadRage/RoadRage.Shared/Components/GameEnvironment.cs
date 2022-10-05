using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadRage.Components
{
    public class GameEnvironment : Canvas
    {
        public GameEnvironment()
        {
            RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
        }
    }
}
