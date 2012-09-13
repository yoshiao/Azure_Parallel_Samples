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
// The next few lines exemplify the Using the Windows Azure Diagnostics Trace Listener recipe
            System.Diagnostics.Trace.Listeners.Add(new Microsoft.WindowsAzure.Diagnostics.DiagnosticMonitorTraceListener());
            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.TraceInformation("TraceInformation - In OnStart");
            System.Diagnostics.Trace.TraceWarning("TraceWarning - In OnStart");
            System.Diagnostics.Trace.TraceError("TraceError - In OnStart");
            System.Diagnostics.Trace.WriteLine("WriteLine - In OnStart");
            
            WadManagement container = new WadManagement();
            container.InitializeConfiguration();

            return base.OnStart();   
        }
    }
}
