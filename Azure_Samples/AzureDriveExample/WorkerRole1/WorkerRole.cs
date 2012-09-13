﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("WorkerRole1 entry point called", "Information");

            AzureDriveExample.UseAzureDriveExample();

            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            String connectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
            DiagnosticMonitorConfiguration configuration = DiagnosticMonitor.GetDefaultInitialConfiguration();
            configuration.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1d);
            configuration.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            DiagnosticMonitor.Start(connectionString, configuration); 

            return base.OnStart();
        }
    }
}
