using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.ConstrainedExecution;

namespace SkyWay
{
    public class Car : GameObject
    {
        public Car()
        {
            Tag = Constants.CAR_TAG;            
        }
    }
}

