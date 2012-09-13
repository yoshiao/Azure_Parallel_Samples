using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;

using System.Net;
using System.IO;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {
        XNamespace wa = "http://schemas.microsoft.com/windowsazure";
        XNamespace sc = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration";

        String changeConfigurationFormat = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}/?comp=config";
        String getConfigurationFormat = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}";

        String subscriptionId = RoleEnvironment.GetConfigurationSettingValue("SubscriptionId");
        String serviceName = RoleEnvironment.GetConfigurationSettingValue("ServiceName");
        String deploymentSlot = RoleEnvironment.GetConfigurationSettingValue("DeploymentSlot");
        String thumbprint = RoleEnvironment.GetConfigurationSettingValue("Thumbprint");
        String roleName = "WebRole1";
        String instanceId = "WebRole1_IN_0";

        public override bool OnStart()
        {
            return base.OnStart();
        }

        public override void Run()
        {
            Int32 countMinutes = 0;
            while (true)
            {
                Thread.Sleep(60000);
                if (++countMinutes == 20)
                {
                    countMinutes = 0;
                    if (RoleEnvironment.CurrentRoleInstance.Id == instanceId)
                    {
                        ChangeInstanceCount();
                    }
                }
            }
        }

        private void ChangeInstanceCount()
        {
            XElement configuration = LoadConfiguration();
            Int32 requiredInstanceCount = CalculateRequiredInstanceCount();
            if (GetInstanceCount(configuration) != requiredInstanceCount)
            {
                SetInstanceCount(configuration, requiredInstanceCount);
                String requestId = SaveConfiguration(configuration);
            }
        }

        private Int32 CalculateRequiredInstanceCount()
        {
            Int32 instanceCount = 2;
            DayOfWeek dayOfWeek = DateTime.UtcNow.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
            {
                instanceCount = 1;
            }
            return instanceCount;
        }

        private Int32 GetInstanceCount(XElement configuration)
        {
            XElement instanceElement = (from s in configuration.Elements(sc + "Role")
                                        where s.Attribute("name").Value == roleName
                                        select s.Element(sc + "Instances")).First();
            Int32 instanceCount = (Int32)Convert.ToInt32(instanceElement.Attribute("count").Value);
            return instanceCount;
        }

        private void SetInstanceCount(XElement configuration, Int32 value)
        {
            XElement instanceElement = (from s in configuration.Elements(sc + "Role")
                                        where s.Attribute("name").Value == roleName
                                        select s.Element(sc + "Instances")).First();
            instanceElement.SetAttributeValue("count", value);
        }

        private XDocument CreatePayload(XElement configuration)
        {
            String configurationString = configuration.ToString();
            String base64Configuration = ConvertToBase64String(configurationString);

            XElement xConfiguration = new XElement(wa + "Configuration", base64Configuration);
            XElement xChangeConfiguration = new XElement(wa + "ChangeConfiguration", xConfiguration);

            XDocument payload = new XDocument();
            payload.Add(xChangeConfiguration);
            payload.Declaration = new XDeclaration("1.0", "UTF-8", "no");

            return payload;
        }

        private XElement LoadConfiguration()
        {
            String uri = String.Format(getConfigurationFormat, subscriptionId, serviceName, deploymentSlot);
            ServiceManagementOperation operation = new ServiceManagementOperation(thumbprint);
            XDocument deployment = operation.Invoke(uri);

            String base64Configuration = deployment.Element(wa + "Deployment").Element(wa + "Configuration").Value;
            String stringConfiguration = ConvertFromBase64String(base64Configuration);

            XElement configuration = XElement.Parse(stringConfiguration);
            return configuration;
        }

        private String SaveConfiguration(XElement configuration)
        {
            String uri = String.Format(changeConfigurationFormat, subscriptionId, serviceName, deploymentSlot);

            XDocument payload = CreatePayload(configuration);
            ServiceManagementOperation operation = new ServiceManagementOperation(thumbprint);
            String requestId = operation.Invoke(uri, payload);
            return requestId;
        }

        private String ConvertToBase64String(String value)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            String base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        private String ConvertFromBase64String(String base64Value)
        {
            Byte[] bytes = Convert.FromBase64String(base64Value);
            String value = System.Text.Encoding.UTF8.GetString(bytes);
            return value;
        }
    }
}
