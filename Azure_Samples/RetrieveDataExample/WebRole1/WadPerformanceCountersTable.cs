using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class WadPerformanceCountersTable
    {
        public static String Name = "WadPerformanceCountersTable";

        public String PartitionKey { get; set; }
        public String RowKey { get; set; }
        public Int64 EventTickCount { get; set; }
        public String DeploymentId { get; set; }
        public String Role { get; set; }
        public String RoleInstance { get; set; }
        public String CounterName { get; set; }
        public Double CounterValue { get; set; }

        public DateTime EventDateTime
        {
            get { return new DateTime(EventTickCount); }
        }
    }
}