using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;

namespace QueueWpf
{
    class LargeDataExample
    {
        private CloudQueue cloudQueue;
        private CloudBlobClient cloudBlobClient;

        public LargeDataExample(String queueName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]);
            CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        public String SetupRecipe(String containerName, String blobName)
        {
            cloudQueue.CreateIfNotExist();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            cloudBlobContainer.CreateIfNotExist();
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            cloudBlockBlob.UploadText("Large amount of data");
            return cloudBlockBlob.Uri.AbsoluteUri;
        }

        public void AddLargeMessage(String blobUrl)
        {
            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(blobUrl);
            cloudQueue.AddMessage(cloudQueueMessage);
        }

        public void ProcessLargeMessage()
        {
            CloudQueueMessage cloudQueueMessage = cloudQueue.GetMessage();
            if (cloudQueueMessage != null)
            {
                String blobUri = cloudQueueMessage.AsString;
                CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobUri);
                String blobText = cloudBlockBlob.DownloadText();
                // Process blobText 
                cloudBlockBlob.Delete();
                cloudQueue.DeleteMessage(cloudQueueMessage);
            }
        }

        public static void UseLargeDataExample()
        {
            String queueName = "largedata";
            String containerName = "largedata";
            String blobName = "largeblob";

            LargeDataExample example = new LargeDataExample(queueName);
            String blobUrl = example.SetupRecipe(containerName, blobName);

            example.AddLargeMessage(blobUrl);
            example.ProcessLargeMessage();
        }
    }
}
