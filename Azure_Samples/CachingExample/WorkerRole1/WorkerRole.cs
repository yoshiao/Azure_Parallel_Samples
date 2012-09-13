using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

using Microsoft.ApplicationServer.Caching;
using System.Security;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        DataCache dataCache;
        String containerName = "CONTAINER_NAME";
        String key = "BLOB_NAME";

        public override void Run()
        {
            while (true)
            {
                Thread.Sleep(10000);
                String theValue = GetData(key);
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            InitializeCache();

            RemoveData(key);

            String value = GetData(key);

            return base.OnStart();
        }


        private void InitializeCache()
        {
            String cacheNamespace = RoleEnvironment.GetConfigurationSettingValue("Namespace");
            String cacheHost = String.Format("{0}.cache.windows.net", cacheNamespace);
            Boolean SslEnabled = true;
            Int32 cachePort = SslEnabled ? 22243 : 22233;
            Int32 sizeLocalCache = 100;

            DataCacheLocalCacheProperties localCacheProperties = new DataCacheLocalCacheProperties(
                sizeLocalCache, TimeSpan.FromSeconds(60), DataCacheLocalCacheInvalidationPolicy.TimeoutBased);

            List<DataCacheServerEndpoint> servers = new List<DataCacheServerEndpoint>();
            servers.Add(new DataCacheServerEndpoint(cacheHost, cachePort));

            DataCacheTransportProperties dataCacheTransportProperties = new DataCacheTransportProperties() { MaxBufferSize = 10000, ReceiveTimeout = TimeSpan.FromSeconds(45) };

            DataCacheFactoryConfiguration cacheFactoryConfiguration = new DataCacheFactoryConfiguration()
            {
                LocalCacheProperties = localCacheProperties,
                SecurityProperties = GetSecurityToken(SslEnabled),
                Servers = servers,
                TransportProperties = dataCacheTransportProperties
            };

            DataCacheFactory dataCacheFactory = new DataCacheFactory(cacheFactoryConfiguration);
            dataCache = dataCacheFactory.GetDefaultCache();
        }


        private DataCacheSecurity GetSecurityToken(Boolean SslEnabled)
        {
            DataCacheSecurity dataCacheSecurity;
            using (SecureString secureString = new SecureString())
            {
                String authenticationToken = RoleEnvironment.GetConfigurationSettingValue("AuthenticationToken");
                foreach (Char c in authenticationToken)
                {
                    secureString.AppendChar(c);
                }
                secureString.MakeReadOnly();
                dataCacheSecurity = new DataCacheSecurity(secureString, SslEnabled);
            }
            return dataCacheSecurity;
        }

        private void RemoveData(String key)
        {
            dataCache.Remove(key);
        }

        private String GetData(String key)
        {
            String blobText = String.Empty;
            blobText = dataCache.Get(key) as String;
            if (String.IsNullOrEmpty(blobText))
            {
                blobText = GetBlobFromStorage(containerName, key);
                dataCache.Put(key, blobText, TimeSpan.FromMinutes(10d));
            }
            return blobText;
        }

        private String GetBlobFromStorage(String container, String blobName)
        {
            String blobAddress = String.Format("{0}/{1}", container, blobName);
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobAddress);
            String blobText = cloudBlockBlob.DownloadText();
            return blobText;
        }
    }
}
