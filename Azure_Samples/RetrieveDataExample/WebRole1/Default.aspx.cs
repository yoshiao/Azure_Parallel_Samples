using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

// System.Data.Services.Client

namespace WebRole1
{
    public partial class _Default : System.Web.UI.Page
    {
        private String wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

        protected void Page_Load(object sender, EventArgs e)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse( RoleEnvironment.GetConfigurationSettingValue( wadConnectionString));
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            DateTime now = DateTime.UtcNow;
            DateTime fiveMinutesAgo = now.AddMinutes(-5);
            String partitionKeyNow = String.Format("0{0}", now.Ticks.ToString()); 
            String partitionKey5MinutesAgo = String.Format("0{0}", fiveMinutesAgo.Ticks.ToString());

            TableServiceContext tableServiceContext = cloudTableClient.GetDataServiceContext();
            CloudTableQuery<WadPerformanceCountersTable> cloudTableQuery =
              (from entity in tableServiceContext.CreateQuery<WadPerformanceCountersTable>(WadPerformanceCountersTable.Name)
               where entity.PartitionKey.CompareTo(partitionKeyNow) < 0 && entity.PartitionKey.CompareTo(partitionKey5MinutesAgo) > 0
               select entity).Take(200).AsTableServiceQuery();

            GridView1.DataSource = cloudTableQuery;
            GridView1.DataBind();
        }
    }
}
