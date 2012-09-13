using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.ServiceBus;
using System.ServiceModel;
using Server;

namespace Client
{
    class Program
    {
        static String serviceNamespace = "SERVICE_NAMESPACE";
        static String issuerName = "owner";
        static String issuerKey = "AUTHENTICATION_TOKEN";
        static String serviceName = "TemperatureService";

        static void Main(string[] args)
        {
            RunClient();
        }

        private static void RunClient()
        {
            Console.WriteLine("Client");

            TransportClientEndpointBehavior credentialBehavior = new TransportClientEndpointBehavior();
            credentialBehavior.CredentialType = TransportClientCredentialType.SharedSecret;
            credentialBehavior.Credentials.SharedSecret.IssuerName = issuerName;
            credentialBehavior.Credentials.SharedSecret.IssuerSecret = issuerKey;

            NetTcpRelayBinding binding = new NetTcpRelayBinding();

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, serviceName);
            EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

            using (ChannelFactory<ITemperatureContract> channelFactory = new ChannelFactory<ITemperatureContract>(binding, endpointAddress))
            {
                channelFactory.Endpoint.Behaviors.Add(credentialBehavior);

                ITemperatureContract channel = channelFactory.CreateChannel();
                ((ICommunicationObject)channel).Open();

                Double boilingPointCelsius = channel.ToCelsius(212);
                Double boilingPointFahrenheit = channel.ToFahrenheit(boilingPointCelsius);

                Console.WriteLine("212 Fahrenheit is {0} Celsius", boilingPointCelsius);
                Console.WriteLine("{0} Celsius is {1} Fahrenheit", boilingPointCelsius, boilingPointFahrenheit);

                ((ICommunicationObject)channel).Close();
            
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
