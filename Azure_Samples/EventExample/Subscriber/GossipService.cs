using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

namespace Subscriber
{
    [ServiceBehavior(Name = "GossipService")]
    public class GossipService : IGossipContract
    {
        public void ShareGossip(String gossip)
        {
            String moreGossip = gossip;
            Console.WriteLine(moreGossip);
        }
    }
}
