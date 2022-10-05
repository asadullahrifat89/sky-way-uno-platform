using System;
using System.Collections.Generic;
using System.Text;

namespace RoadRage
{
    public static class Constants
    {
        public const string PLAYER_TAG = "player";

        public const string CAR_TAG = "car";
        public const string TRUCK_TAG = "truck";

        public const string POWERUP_TAG = "powerup";
        public const string HEALTH_TAG = "health";

        public const string ROADMARK_TAG = "roadmark";

        public static Uri[] CAR_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/car1.png"),
            new Uri("ms-appx:///Assets/Images/car2.png"),
            new Uri("ms-appx:///Assets/Images/car3.png"),
            new Uri("ms-appx:///Assets/Images/car4.png"),
            new Uri("ms-appx:///Assets/Images/car5.png"),
            new Uri("ms-appx:///Assets/Images/car6.png")
        };

        public static Uri[] TRUCK_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/truck1.png"),
            new Uri("ms-appx:///Assets/Images/truck2.png"),
            new Uri("ms-appx:///Assets/Images/truck3.png"),
            new Uri("ms-appx:///Assets/Images/truck4.png"),
            new Uri("ms-appx:///Assets/Images/truck5.png"),
        };

        public static Uri[] ROADMARK_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/road-mark1.png"),
            new Uri("ms-appx:///Assets/Images/road-mark2.png"),
            new Uri("ms-appx:///Assets/Images/road-mark3.png"),
        };

        public static Uri[] TREE_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/tree1.png"),
            new Uri("ms-appx:///Assets/Images/tree2.png"),
            new Uri("ms-appx:///Assets/Images/tree3.png"),
            new Uri("ms-appx:///Assets/Images/tree4.png"),
        };

        public static Uri POWERUP_TEMPLATE = new Uri("ms-appx:///Assets/Images/powerup.gif");
        public static Uri HEALTH_TEMPLATE = new Uri("ms-appx:///Assets/Images/health.gif");
    }
}
