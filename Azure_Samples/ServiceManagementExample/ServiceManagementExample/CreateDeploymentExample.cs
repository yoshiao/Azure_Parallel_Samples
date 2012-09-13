using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.IO;

namespace ServiceManagementExample
{
    class CreateDeploymentExample
    {
        XNamespace wa = "http://schemas.microsoft.com/windowsazure";
        String createDeploymentFormat = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}";
        String getOperationStatusFormat = "https://management.core.windows.net/{0}/operations/{1}";

        private String ConvertToBase64String(String value)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            String base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        private XDocument CreatePayload(String deploymentName, String packageUrl, String pathToConfigurationFile, String label)
        {
            String configurationFile = File.ReadAllText(pathToConfigurationFile);
            String base64ConfigurationFile = ConvertToBase64String(configurationFile);

            String base64Label = ConvertToBase64String(label);

            XElement xName = new XElement(wa + "Name", deploymentName);
            XElement xPackageUrl = new XElement(wa + "PackageUrl", packageUrl);
            XElement xLabel = new XElement(wa + "Label", base64Label);
            XElement xConfiguration = new XElement(wa + "Configuration", base64ConfigurationFile);
            XElement xStartDeployment = new XElement(wa + "StartDeployment", "true");
            XElement xTreatWarningsAsError = new XElement(wa + "TreatWarningsAsError", "false");
            XElement createDeployment = new XElement(wa + "CreateDeployment");

            createDeployment.Add(xName);
            createDeployment.Add(xPackageUrl);
            createDeployment.Add(xLabel);
            createDeployment.Add(xConfiguration);
            createDeployment.Add(xStartDeployment);
            createDeployment.Add(xTreatWarningsAsError);

            XDocument payload = new XDocument();
            payload.Add(createDeployment);
            payload.Declaration = new XDeclaration("1.0", "UTF-8", "no");

            return payload;
        }

        private String CreateDeployment(String subscriptionId, String thumbprint, String serviceName, String deploymentName, String deploymentSlot, String packageUrl, String pathToConfigurationFile, String label)
        {
            String uri = String.Format(createDeploymentFormat, subscriptionId, serviceName, deploymentSlot);

            XDocument payload = CreatePayload(deploymentName, packageUrl, pathToConfigurationFile, label);
            ServiceManagementOperation operation = new ServiceManagementOperation(thumbprint);
            String requestId = operation.Invoke(uri, payload);
            return requestId;
        }

        public String GetOperationStatus(String subscriptionId, String thumbprint, String requestId)
        {
            String uri = String.Format(getOperationStatusFormat, subscriptionId, requestId);
            ServiceManagementOperation operation = new ServiceManagementOperation(thumbprint);
            XDocument operationStatus = operation.Invoke(uri);

            String status = operationStatus.Element(wa + "Operation").Element(wa + "Status").Value;

            return status;
        }

        public static void UseCreateDeploymentExample()
        {
            String subscriptionId = "SUBSCRIPTION_ID";
            String thumbprint = "THUMBPRINT";
            String serviceName = "SERVICE_NAME";
            String deploymentName = "DEPLOYMENT_NAME";
            String deploymentSlot = "production";
            String packageUrl = "PACKAGE_URL";
            String pathToConfigurationFile = "ServiceConfiguration.cscfg";
            String label = "LABEL";
            CreateDeploymentExample example = new CreateDeploymentExample();
            String requestId = example.CreateDeployment(subscriptionId, thumbprint, serviceName, deploymentName, deploymentSlot, packageUrl, pathToConfigurationFile, label);
            String operationStatus = example.GetOperationStatus(subscriptionId, thumbprint, requestId);
        }
    }
}
