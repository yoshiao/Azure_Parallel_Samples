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

using System.IO;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private String localResourceName = "CustomLoggingLocation";

        public override void Run()
        {
            Trace.WriteLine("WorkerRole1 entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");

                CreateLogFile();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            InitializeWadConfiguration();

            return base.OnStart();
        }

        private void InitializeWadConfiguration()
        {
            String wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
            String customContainerName = "wad-custom-container";

            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();

            LocalResource localResource = RoleEnvironment.GetLocalResource(localResourceName);
            String logPath = Path.Combine(localResource.RootPath, "Logs");
            DirectoryConfiguration directoryConfiguration = new DirectoryConfiguration()
            {
                Container = customContainerName,
                DirectoryQuotaInMB = localResource.MaximumSizeInMegabytes,
                Path = logPath
            };
            dmc.Directories.DataSources.Add(directoryConfiguration);
            dmc.Directories.ScheduledTransferPeriod = TimeSpan.FromHours(1);

            dmc.Logs.BufferQuotaInMB = 100;
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromHours(1);
            dmc.Logs.ScheduledTransferLogLevelFilter =  LogLevel.Verbose;

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));
            DiagnosticMonitor.Start(wadConnectionString, dmc);
        }

        private void CreateLogFile()
        {
            LocalResource localResource = RoleEnvironment.GetLocalResource(localResourceName);
            String logPath = Path.Combine(localResource.RootPath, "Logs");
            String fileName = Path.Combine(logPath, Path.GetRandomFileName());

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                streamWriter.Write("If we shadows have offended");
            }
        }

    }
}
