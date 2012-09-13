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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["OriginalInstanceId"] == null) 
            {
                Session["OriginalInstanceId"] = RoleEnvironment.CurrentRoleInstance.Id;
            }
            SessionId.Text = Session.SessionID;
            OriginalInstance.Text = Session["OriginalInstanceId"].ToString();
            CurrentInstance.Text = RoleEnvironment.CurrentRoleInstance.Id;
        }
    }
}
