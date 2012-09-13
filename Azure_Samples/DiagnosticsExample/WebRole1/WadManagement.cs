using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace WebRole1
{
    public class WadManagement
    {
        private String wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private String wadControlContainerName = "wad-control-container";

        private CloudStorageAccount cloudStorageAccount;

        public WadManagement()
        {
            cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));
        }

        public String GetConfigurationBlob(String deploymentId, String roleName, String instanceId)
        {
            DeploymentDiagnosticManager deploymentDiagnosticManager = new DeploymentDiagnosticManager(cloudStorageAccount, deploymentId);

            String wadConfigurationBlobNameForInstance = String.Format("{0}/{1}/{2}", deploymentId, roleName, instanceId);
            String wadConfigurationForInstance = GetWadConfigurationForInstance(wadConfigurationBlobNameForInstance);

            return wadConfigurationForInstance;
        }


        private String GetWadConfigurationForInstance(String wadConfigurationInstanceBlobName)
        {
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(wadControlContainerName);
            CloudBlob cloudBlob = cloudBlobContainer.GetBlobReference(wadConfigurationInstanceBlobName);

            String wadConfigurationForInstance = cloudBlob.DownloadText();

            return wadConfigurationForInstance;
        }


        public void InitializeConfiguration()
        {
            String eventLog = "Application!*";
            String performanceCounter = @"\Processor(_Total)\% Processor Time";

            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();

            dmc.DiagnosticInfrastructureLogs.BufferQuotaInMB = 100;
            dmc.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TimeSpan.FromHours(1);
            dmc.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

            dmc.WindowsEventLog.BufferQuotaInMB = 100;
            dmc.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            dmc.WindowsEventLog.DataSources.Add(eventLog);

            dmc.Logs.BufferQuotaInMB = 100;
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromHours(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

            dmc.Directories.ScheduledTransferPeriod = TimeSpan.FromHours(1);

            PerformanceCounterConfiguration perfCounterConfiguration = new PerformanceCounterConfiguration();
            perfCounterConfiguration.CounterSpecifier = performanceCounter;
            perfCounterConfiguration.SampleRate = System.TimeSpan.FromSeconds(10);
            dmc.PerformanceCounters.DataSources.Add(perfCounterConfiguration);
            dmc.PerformanceCounters.BufferQuotaInMB = 100;
            dmc.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromHours(1);

            DiagnosticMonitor.Start(cloudStorageAccount, dmc);
        }


        public void ModifyConfiguration(String deploymentId, String roleName, String instanceId)
        {
            String eventLog = @"Application!*[System[Provider[@Name='.NET Runtime']]]";
            String performanceCounter = @"\ASP.NET\Requests Rejected";

            RoleInstanceDiagnosticManager ridm = cloudStorageAccount.CreateRoleInstanceDiagnosticManager(deploymentId, roleName, instanceId);
            DiagnosticMonitorConfiguration dmc = ridm.GetCurrentConfiguration();

            Int32 countDataSources = dmc.WindowsEventLog.DataSources.Count(item => item == eventLog);
            if (countDataSources == 0)
            {
                dmc.WindowsEventLog.DataSources.Add(eventLog);
                dmc.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromHours(1);
            }

            countDataSources = dmc.PerformanceCounters.DataSources.Count(item => item.CounterSpecifier == performanceCounter);
            if (countDataSources == 0)
            {
                PerformanceCounterConfiguration perfCounterConfiguration = new PerformanceCounterConfiguration()
                {
                    CounterSpecifier = performanceCounter,
                    SampleRate = System.TimeSpan.FromHours(1)
                };
                dmc.PerformanceCounters.DataSources.Add(perfCounterConfiguration);
            }

            IDictionary<DataBufferName, OnDemandTransferInfo> activeTransfers = ridm.GetActiveTransfers();
            if (activeTransfers.Count == 0)
            {
                ridm.SetCurrentConfiguration(dmc);
            }
        }

    }
}