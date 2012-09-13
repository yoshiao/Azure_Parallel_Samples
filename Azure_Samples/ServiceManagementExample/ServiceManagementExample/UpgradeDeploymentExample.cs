using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.IO;

namespace ServiceManagementExample
{
    class UpgradeDeploymentExample
    {
        XNamespace wa = "http://schemas.microsoft.com/windowsazure";
        String upgradeDeploymentFormat = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}/?comp=upgrade";

        private String ConvertToBase64String(String value)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            String base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        private XDocument CreatePayload(String roleName, String packageUrl, String pathToConfigurationFile, String label)
        {
            String configurationFile = File.ReadAllText(pathToConfigurationFile);
            String base64ConfigurationFile = ConvertToBase64String(configurationFile);

            String base64Label = ConvertToBase64String(label);

            XElement xMode = new XElement(wa + "Mode", "auto");
            XElement xPackageUrl = new XElement(wa + "PackageUrl", packageUrl);
            XElement xConfiguration = new XElement(wa + "Configuration", base64ConfigurationFile);
            XElement xLabel = new XElement(wa + "Label", base64Label);
            XElement xRoleToUpgrade = new XElement(wa + "RoleToUpgrade", roleName);
            XElement upgradeDeployment = new XElement(wa + "UpgradeDeployment");

            upgradeDeployment.Add(xMode);
            upgradeDeployment.Add(xPackageUrl);
            upgradeDeployment.Add(xConfiguration);
            upgradeDeployment.Add(xLabel);
            upgradeDeployment.Add(xRoleToUpgrade);

            XDocument payload = new XDocument();
            payload.Add(upgradeDeployment);
            payload.Declaration = new XDeclaration("1.0", "UTF-8", "no");

            return payload;
        }

        private String UpgradeDeployment(String subscriptionId, String thumbprint, String serviceName, String deploymentSlot, String roleName, String packageUrl, String pathToConfigurationFile, String label )
        {
            String uri = String.Format(upgradeDeploymentFormat, subscriptionId, serviceName, deploymentSlot);

            XDocument payload = CreatePayload(roleName, packageUrl, pathToConfigurationFile, label);
            ServiceManagementOperation operation = new ServiceManagementOperation(thumbprint);
            String requestId = operation.Invoke(uri, payload);
            return requestId;
        }

        public static void UseUpgradeDeploymentExample()
        {
            String subscriptionId = "SUBSCRIPTION_ID";
            String thumbprint = "THUMBPRINT";
            String serviceName = "SERVICE_NAME";
            String deploymentSlot = "production";
            String roleName = "WebRole1";
            String packageUrl = "PACKAGE_URL";
            String pathToConfigurationFile = "ServiceConfiguration.cscfg";
            String label = "LABEL";
            UpgradeDeploymentExample example = new UpgradeDeploymentExample();
            String requestId = example.UpgradeDeployment(subscriptionId, thumbprint, serviceName, deploymentSlot, roleName, packageUrl, pathToConfigurationFile, label );
        }
    }
}
