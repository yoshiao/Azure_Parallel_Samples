using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;

namespace QueueWpf
{
    class GetMessagesExample
    {
        private CloudQueue cloudQueue;

        public GetMessagesExample(String queueName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]);
            CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
        }

        public void SetupRecipe()
        {
            cloudQueue.CreateIfNotExist();
            for (Int32 i = 0; i < 100; i++)
            {
                String content = String.Format("Message_{0}", i);
                CloudQueueMessage message = new CloudQueueMessage(content);
                cloudQueue.AddMessage(message);
            }
        }

        public void GetMessage()
        {
            Int32 messageCount = cloudQueue.RetrieveApproximateMessageCount();
            CloudQueueMessage cloudQueueMessage = cloudQueue.GetMessage();
            if (cloudQueueMessage != null)
            {
                String messageText = cloudQueueMessage.AsString;
                // use message
                cloudQueue.DeleteMessage(cloudQueueMessage);
            }
        }

        public void PeekMessage()
        {
            CloudQueueMessage cloudQueueMessage = cloudQueue.PeekMessage();
        }

        public void GetMessages()
        {
            IEnumerable<CloudQueueMessage> cloudQueueMessages = cloudQueue.GetMessages(20);
            foreach (CloudQueueMessage message in cloudQueueMessages)
            {
                String messageText = message.AsString;
                // use message
                cloudQueue.DeleteMessage(message);
            }
        }

        public static void UseGetMessagesExample()
        {
            String queueName = "actions";
            GetMessagesExample example = new GetMessagesExample(queueName);
            example.SetupRecipe();
            example.GetMessage();
            example.PeekMessage();
            example.GetMessages();
        }
    }
}
