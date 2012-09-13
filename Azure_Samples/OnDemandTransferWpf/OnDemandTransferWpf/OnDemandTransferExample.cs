using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;


namespace OnDemandTransferWpf
{
    class OnDemandTransferExample
    {
        String wadNotificationQueueName = "wad-transfer-queue";

        public void RequestOnDemandTransfer( String deploymentId, String roleName, String roleInstanceId)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DiagnosticsConnectionString"]);

            OnDemandTransferOptions onDemandTransferOptions = new OnDemandTransferOptions()
            {
                From = DateTime.UtcNow.AddHours(-1),
                To = DateTime.UtcNow,
                LogLevelFilter = Microsoft.WindowsAzure.Diagnostics.LogLevel.Verbose,
                NotificationQueueName = wadNotificationQueueName
            };

            RoleInstanceDiagnosticManager ridm = cloudStorageAccount.CreateRoleInstanceDiagnosticManager(deploymentId, roleName, roleInstanceId);
            IDictionary<DataBufferName, OnDemandTransferInfo> activeTransfers = ridm.GetActiveTransfers();
            if (activeTransfers.Count == 0)
            {
                Guid onDemandTransferId = ridm.BeginOnDemandTransfer( DataBufferName.PerformanceCounters, onDemandTransferOptions);
            }
        }

        public void CleanupOnDemandTransfers()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DiagnosticsConnectionString"]);
            CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference( wadNotificationQueueName);
            CloudQueueMessage cloudQueueMessage;
            while ((cloudQueueMessage =  cloudQueue.GetMessage()) != null)
            {
                OnDemandTransferInfo onDemandTransferInfo = OnDemandTransferInfo.FromQueueMessage(cloudQueueMessage);
                String deploymentId = onDemandTransferInfo.DeploymentId;
                String roleName = onDemandTransferInfo.RoleName;
                String roleInstanceId = onDemandTransferInfo.RoleInstanceId;
                Guid requestId = onDemandTransferInfo.RequestId;

                RoleInstanceDiagnosticManager ridm = cloudStorageAccount.CreateRoleInstanceDiagnosticManager(deploymentId, roleName, roleInstanceId);
                Boolean result = ridm.EndOnDemandTransfer(requestId);
                cloudQueue.DeleteMessage(cloudQueueMessage);
            }
        }
    }
}
