using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            ConfigureDiagnostics();

            return base.OnStart();
        }

        private void ConfigureDiagnostics()
        {
            String wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));

            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();

            PerformanceCounterConfiguration pmc = new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                SampleRate = System.TimeSpan.FromSeconds(10)
            };
            dmc.PerformanceCounters.DataSources.Add( pmc);
            dmc.PerformanceCounters.BufferQuotaInMB = 100;
            dmc.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

            DiagnosticMonitor.Start(cloudStorageAccount, dmc);
        }

    }


}
