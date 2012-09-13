using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

namespace Server
{
    [ServiceBehavior(Name = "TemperatureService")]
    public class TemperatureService : ITemperatureContract
    {
        public Double ToCelsius(Double fahrenheit)
        {
            Double celsius = (fahrenheit - 32d) * 5d / 9d;
            Console.WriteLine("{0} Fahrenheit is {1} Celsius", fahrenheit, celsius);
            return celsius;
        }

        public Double ToFahrenheit(Double celsius)
        {
            Double fahrenheit = 32d + celsius * 9d / 5d;
            Console.WriteLine("{0} Celsius is {1} Fahrenheit", celsius, fahrenheit);
            return fahrenheit;
        }
    }
}
