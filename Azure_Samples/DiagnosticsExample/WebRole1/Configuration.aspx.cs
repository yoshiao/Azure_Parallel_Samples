using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;

namespace WebRole1
{
    public partial class Configuration : System.Web.UI.Page
    {
        private String deploymentId = RoleEnvironment.DeploymentId;
        private String roleName = RoleEnvironment.CurrentRoleInstance.Role.Name;
        private String instanceId = RoleEnvironment.CurrentRoleInstance.Id;

        protected void Page_Load(object sender, EventArgs e)
        {
            WadManagement container = new WadManagement();
            container.ModifyConfiguration(deploymentId, roleName, instanceId);
            String wadConfigurationForInstance = container.GetConfigurationBlob(deploymentId, roleName, instanceId);
            xmlLabel.Text = Server.HtmlEncode(wadConfigurationForInstance);
        }
    }
}