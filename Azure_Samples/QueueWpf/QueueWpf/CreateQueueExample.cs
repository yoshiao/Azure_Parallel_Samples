using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;

namespace QueueWpf
{
    class CreateQueueExample
    {
        private CloudQueueClient cloudQueueClient;

        public CreateQueueExample()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]);
            cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
        }

        public void CreateQueue(String queueName)
        {
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.Create();
        }

        public void GetQueueInformation(String queueName)
        {
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            Int32 approximateMessageCount = cloudQueue.RetrieveApproximateMessageCount();
            cloudQueue.FetchAttributes();
            Uri uri = cloudQueue.Attributes.Uri;
            foreach ( String key in cloudQueue.Metadata.Keys)
            {
                String metadataValue = cloudQueue.Metadata[key];
            }
        }

        public void DeleteQueue(String queueName)
        {
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.Delete();
        }

        public static void UseCreateQueueExample()
        {
            String queueName = "actions1";
            CreateQueueExample example = new CreateQueueExample();
            example.CreateQueue(queueName);
            example.GetQueueInformation(queueName);
            example.DeleteQueue(queueName);
        }
    }
}
