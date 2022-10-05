using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.ConstrainedExecution;

namespace RoadRage
{
    public class Car : GameObject
    {
        public Car()
        {
            Tag = Constants.CAR_TAG;
            var carNum = new Random().Next(0, Constants.CAR_TEMPLATES.Length);
            SetContent(Constants.CAR_TEMPLATES[carNum]);
        }
    }
}

