using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;

namespace QueueWpf
{

    class AddMessagesExample
    {
        private CloudQueue cloudQueue;

        public AddMessagesExample( String queueName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]);
            CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.CreateIfNotExist();
        }


        public void AddMessages()
        {
            String content1 = "Do something";
            CloudQueueMessage message1 = new CloudQueueMessage(content1);
            cloudQueue.AddMessage(message1);

            String content2 = "Do something else";
            CloudQueueMessage message2 = new CloudQueueMessage(content2);
            cloudQueue.AddMessage(message2, TimeSpan.FromDays(1.0));
        }

        public static void UseAddMessagesExample()
        {
            String queueName = "actions";
            AddMessagesExample example = new AddMessagesExample(queueName);
            example.AddMessages();
        }
    }
}
