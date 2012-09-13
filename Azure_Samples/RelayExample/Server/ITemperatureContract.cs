using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

namespace Server
{
    [ServiceContract(Name = "ITemperatureContract")]
    interface ITemperatureContract
    {
        [OperationContract]
        Double ToCelsius(Double fahrenheit);

        [OperationContract]
        Double ToFahrenheit(Double celsius);
    }
}
