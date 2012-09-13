using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Web.Administration;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            // Uncomment the following line to set the application pool idle timout in code
            //SetIdleTimeout(TimeSpan.FromMinutes(80d));

            return base.OnStart();
        }

        private void SetIdleTimeout(TimeSpan timeout)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                serverManager.ApplicationPoolDefaults.ProcessModel.IdleTimeout = timeout;
                serverManager.CommitChanges();
            }
        }
    }
}
