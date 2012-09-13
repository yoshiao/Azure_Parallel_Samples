using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

namespace Subscriber
{
    [ServiceContract(Name = "IGossipContract")]
    public interface IGossipContract
    {
        [OperationContract(IsOneWay = true)]
        void ShareGossip( String gossip);
    }
}
