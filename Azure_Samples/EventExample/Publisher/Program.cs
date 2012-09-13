using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Description;
using System.ServiceModel;
using Subscriber;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            PublishGossip();
        }

        private static void PublishGossip()
        {
            Console.WriteLine("Publisher");

            using (ChannelFactory<IGossipContract> channelFactory = new ChannelFactory<IGossipContract>("RelayEndpoint"))
            {
                IGossipContract channel = channelFactory.CreateChannel();
                ((ICommunicationObject)channel).Open();

                channel.ShareGossip("Gregory, o' my word, we'll not carry coals.");
                channel.ShareGossip("No, for then we should be colliers.");
                channel.ShareGossip("I mean, an we be in choler, we'll draw.");
                channel.ShareGossip("Ay, while you live, draw your neck out o' the collar.");
                channel.ShareGossip("I strike quickly, being moved.");
                channel.ShareGossip("But thou art not quickly moved to strike.");

                ((ICommunicationObject)channel).Close();

                Console.WriteLine("Gossip sent");
                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
            }
        }
    }
}
