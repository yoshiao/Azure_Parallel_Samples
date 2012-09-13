using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebRole1
{
    public partial class _Default : System.Web.UI.Page
    {
        private String deploymentId = RoleEnvironment.DeploymentId;
        private String roleName = RoleEnvironment.CurrentRoleInstance.Role.Name;
        private String instanceId = RoleEnvironment.CurrentRoleInstance.Id;

        protected void Page_Load(object sender, EventArgs e)
        {
            WadManagement wad = new WadManagement();
            String wadConfigurationForInstance = wad.GetConfigurationBlob(deploymentId, roleName, instanceId);
            xmlLabel.Text = Server.HtmlEncode(wadConfigurationForInstance);
        }
    }
}
