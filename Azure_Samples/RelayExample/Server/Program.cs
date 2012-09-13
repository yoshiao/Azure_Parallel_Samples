using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.ServiceBus;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Server
{
    class Program
    {
        static String serviceBusNamespace = "SERVICE_NAMESPACE";
        static String issuerName = "owner";
        static String issuerKey = "AUTHENTICATION_TOKEN";
        static String serviceName = "TemperatureService";

        static void Main(string[] args)
        {
            RunService();
        }

        private static void RunService()
        {
            Console.WriteLine("Server");

            ServiceRegistrySettings registryBehavior = new ServiceRegistrySettings()
            {
                DiscoveryMode = DiscoveryType.Public,
                DisplayName = "Temperature Service"
            };

            TransportClientEndpointBehavior credentialBehavior = new TransportClientEndpointBehavior();
            credentialBehavior.CredentialType = TransportClientCredentialType.SharedSecret;
            credentialBehavior.Credentials.SharedSecret.IssuerName = issuerName;
            credentialBehavior.Credentials.SharedSecret.IssuerSecret = issuerKey;

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, serviceName);
            using (ServiceHost serviceHost = new ServiceHost(typeof(TemperatureService), serviceUri))
            {
                NetTcpRelayBinding binding = new NetTcpRelayBinding();

                serviceHost.AddServiceEndpoint(typeof(ITemperatureContract), binding, serviceUri);
                serviceHost.Description.Endpoints[0].Behaviors.Add(credentialBehavior);
                serviceHost.Description.Endpoints[0].Behaviors.Add(registryBehavior);

                serviceHost.Open();
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
