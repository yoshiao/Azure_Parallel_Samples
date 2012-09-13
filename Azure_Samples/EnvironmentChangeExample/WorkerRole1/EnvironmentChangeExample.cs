using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure.ServiceRuntime;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WorkerRole1
{
    class EnvironmentChangeExample
    {
        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            Boolean recycle = false;
            foreach (RoleEnvironmentChange change in e.Changes)
            {
                RoleEnvironmentTopologyChange topologyChange = change as RoleEnvironmentTopologyChange;
                if (topologyChange != null)
                {
                    String roleName = topologyChange.RoleName;
                    ReadOnlyCollection<RoleInstance> oldInstances = RoleEnvironment.Roles[roleName].Instances;
                }
                RoleEnvironmentConfigurationSettingChange settingChange = change as RoleEnvironmentConfigurationSettingChange;
                if (settingChange != null)
                {
                    String settingName = settingChange.ConfigurationSettingName;
                    String oldValue = RoleEnvironment.GetConfigurationSettingValue(settingName);
                    recycle |= settingName == "SettingRequiringRecycle";
                }
            }

            // Recycle when e.Cancel = true;
            e.Cancel = recycle;
        }

        private static void RoleEnvironmentChanged(object sender, RoleEnvironmentChangedEventArgs e)
        {
            foreach (RoleEnvironmentChange change in e.Changes)
            {
                RoleEnvironmentTopologyChange topologyChange = change as RoleEnvironmentTopologyChange;
                if (topologyChange != null)
                {
                    String roleName = topologyChange.RoleName;
                    ReadOnlyCollection<RoleInstance> newInstances = RoleEnvironment.Roles[roleName].Instances;
                }
                RoleEnvironmentConfigurationSettingChange settingChange = change as RoleEnvironmentConfigurationSettingChange;
                if (settingChange != null)
                {
                    String settingName = settingChange.ConfigurationSettingName;
                    String newValue = RoleEnvironment.GetConfigurationSettingValue(settingName);
                }
            }
        }

        private static void RoleEnvironmentStatusCheck(object sender, RoleInstanceStatusCheckEventArgs e)
        {
            RoleInstanceStatus status = e.Status;
            // Uncomment next line to take instance out of the load balancer rotation.
            //e.SetBusy();
        }

        private static void RoleEnvironmentStopping(object sender, RoleEnvironmentStoppingEventArgs e)
        {
            Trace.TraceInformation("In RoleEnvironmentStopping");
        }

        public static void UseEnvironmentChangeExample()
        {
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.Changed += RoleEnvironmentChanged;
            RoleEnvironment.StatusCheck += RoleEnvironmentStatusCheck;
            RoleEnvironment.Stopping += RoleEnvironmentStopping;
        }
    }
}
