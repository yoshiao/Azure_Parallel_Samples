using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace QueueWpf
{
    [Serializable]
    class SomeClass
    {
        public String Action { get; set; }

        public SomeClass() { }

        public static SomeClass ToSomeClass(Byte[] bytes)
        {
            SomeClass someObject = new SomeClass();
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                someObject = formatter.Deserialize(memoryStream) as SomeClass;
            }
            return someObject;
        }

        public static Byte[] ToByte(SomeClass someObject)
        {
            Byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, someObject);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }
    }

    class PoisonMessagesExample
    {
        private CloudQueue cloudQueue;

        public PoisonMessagesExample(String queueName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]);
            CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.CreateIfNotExist();

            CloudQueue poisonQueue = cloudQueueClient.GetQueueReference("poisoned");
            poisonQueue.CreateIfNotExist();
        }

        public void AddMessage()
        {
            SomeClass someObject = new SomeClass() { Action = "MonteCarloSimulation" };

            Byte[] message = SomeClass.ToByte(someObject);

            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(message);
            cloudQueue.AddMessage(cloudQueueMessage);
        }

        public void AddPoisonMessage()
        {
            String message = "Poison message";
            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(message);
            cloudQueue.AddMessage(cloudQueueMessage);
        }

        public void ProcessMessage()
        {
            CloudQueueMessage cloudQueueMessage = cloudQueue.GetMessage(TimeSpan.FromSeconds(1));
            if (cloudQueueMessage == null)
            {
                return;
            }
            SomeClass someObject;
            try
            {
                someObject = SomeClass.ToSomeClass(cloudQueueMessage.AsBytes);
                // use message
                cloudQueue.DeleteMessage(cloudQueueMessage);
            }
            catch 
            {
                Int32 dequeueCount = cloudQueueMessage.DequeueCount;
                if (dequeueCount > 3)
                {
                    CloudQueueClient cloudQueueClient = cloudQueue.ServiceClient;
                    CloudQueue poisonQueue = cloudQueueClient.GetQueueReference("poisoned");
                    poisonQueue.AddMessage(cloudQueueMessage);
                    cloudQueue.DeleteMessage(cloudQueueMessage);
                }
            }
        }

        public static void UsePoisonMessagesExample()
        {
            String queueName = "tasks";
            PoisonMessagesExample example = new PoisonMessagesExample(queueName);
            example.AddMessage();
            example.AddPoisonMessage();
            for (Int32 i = 0; i < 10; i++)
            {
                example.ProcessMessage();
                Thread.Sleep(2000);
            }
        }

    }
}
