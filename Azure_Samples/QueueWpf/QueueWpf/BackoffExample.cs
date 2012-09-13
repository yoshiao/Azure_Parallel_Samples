using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
using System.Threading;


namespace QueueWpf
{
    class BackoffExample
    {
        public const Int32 minimumBackoff = 10;
        public const Int32 maximumBackoff = 10000;

        private CloudQueue cloudQueue;

        private Int32 backoff = minimumBackoff;
        public Int32 Backoff
        {
            get { return backoff; }
            set {
                backoff = value > maximumBackoff ? maximumBackoff : value;
                backoff = backoff < minimumBackoff ? minimumBackoff : backoff;
            }
        }

        public BackoffExample(String queueName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]);
            CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.CreateIfNotExist();

            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage("Some message");
            cloudQueue.AddMessage(cloudQueueMessage);
        }

        public Boolean ProcessMessage()
        {
            CloudQueueMessage cloudQueueMessage = cloudQueue.GetMessage();
            if (cloudQueueMessage != null)
            {
                String messageText = cloudQueueMessage.AsString;
                // use message
                cloudQueue.DeleteMessage(cloudQueueMessage);
            }

            return cloudQueueMessage != null;
        }


        public static void UseBackoffExample()
        {
            String queueName = "backoff";
            BackoffExample example = new BackoffExample(queueName);

            Int32 noMessageCount = 0;
            for (Int32 i = 0; i < 100; i++)
            {
                if (example.ProcessMessage())
                {
                    example.Backoff = BackoffExample.minimumBackoff;
                    noMessageCount = 0;
                }
                else
                {
                    if (++noMessageCount > 1)
                    {
                        example.Backoff *= 10;
                        noMessageCount = 0;
                    }
                }
                Thread.Sleep(example.Backoff);
            }
        }

    }
}
