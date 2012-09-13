using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using System.Diagnostics;

namespace WorkerRole1
{
    class AzureDriveExample
    {
        private CloudDrive cloudDrive;
        private Int32 cacheSizeInMegabytes;

        public AzureDriveExample(String containerName, String pageBlobName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            cloudBlobContainer.CreateIfNotExist();

            Char[] forwardSlash = { '/' };
            String trimmedUri = cloudStorageAccount.BlobEndpoint.ToString().TrimEnd(forwardSlash);
            String pageBlobUri = String.Format("{0}/{1}/{2}", trimmedUri, containerName, pageBlobName);
            cloudDrive = cloudStorageAccount.CreateCloudDrive(pageBlobUri);
        }


        public void CreateDrive()
        {
            Int32 driveSize = 16; //MB
            try
            {
                cloudDrive.Create(driveSize);
            }
            catch (CloudDriveException e)
            {
                Trace.TraceError("Create error: " + e.Message);
            }
        }
        
        public void InitializeCache()
        {
            LocalResource localCache = RoleEnvironment.GetLocalResource("AzureDriveCache");
            String localCachePath = localCache.RootPath;
            cacheSizeInMegabytes = localCache.MaximumSizeInMegabytes;
            CloudDrive.InitializeCache(localCachePath, cacheSizeInMegabytes);
        }

        public void MountDrive()
        {
            try
            {
                String driveLetter = cloudDrive.Mount(cacheSizeInMegabytes, DriveMountOptions.None);
            }
            catch (CloudDriveException e)
            {
                Trace.TraceError("MountDrive error: " + e.Message);
            }
        }

        public void WriteToDrive(String fileName)
        {
            String path = Path.Combine(cloudDrive.LocalPath, fileName);
            Trace.TraceInformation("path: " + path);
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write("has been changed glorious summer");
            }
        }

        public void ReadFromDrive(String fileName)
        {
            String path = Path.Combine(cloudDrive.LocalPath, fileName);
            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                String text = streamReader.ReadToEnd();
                Trace.TraceInformation("ReadFromDrive: " + text);
            }
        }

        public void UnmountDrive()
        {
            cloudDrive.Unmount();
        }

        public static void UseAzureDriveExample()
        {
            String containerName = "chapter5a";
            String pageBlobName = "Chapter5aVhd";
            String fileName = "Latest.txt";

            AzureDriveExample example = new AzureDriveExample(containerName, pageBlobName);
            example.CreateDrive();
            example.InitializeCache();
            example.MountDrive();
            example.WriteToDrive(fileName);
            example.ReadFromDrive(fileName);
            example.UnmountDrive();
        }
    }
}
