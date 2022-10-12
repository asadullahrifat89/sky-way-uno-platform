using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.ConstrainedExecution;

namespace SkyRacerGame
{
    public class Car : GameObject
    {
        public Car()
        {
            Tag = ElementType.CAR;  
        }
    }
}

