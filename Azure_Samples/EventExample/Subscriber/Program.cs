using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Description;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Subscriber
{
    class Program
    {
        static String serviceBusNamespace = "SERVICE_NAMESPACE";
        static String serviceName = "Subscriber.GossipService";

        static void Main(string[] args)
        {
            SubscribeToGossip();
        }


        private static void SubscribeToGossip()
        {
            Console.WriteLine("Subscriber");

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, serviceName);
            using (ServiceHost serviceHost = new ServiceHost(typeof(GossipService), serviceUri))
            {
                serviceHost.Open();

                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
            }
        }
    }
}
