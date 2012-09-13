using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

namespace ServiceManagementExample
{
    class GetHostedServicePropertiesExample
    {
        XNamespace wa = "http://schemas.microsoft.com/windowsazure";
        String serviceOperationFormat = "https://management.core.windows.net/{0}/services/hostedservices/{1}?embed-detail=true";

        private void GetHostedServiceProperties(String subscriptionId, String thumbprint, String serviceName)
        {
            String uri = String.Format(serviceOperationFormat, subscriptionId, serviceName);
            ServiceManagementOperation operation = new ServiceManagementOperation(thumbprint); 
            XDocument hostedServiceProperties = operation.Invoke(uri);

            var deploymentInformation = (from t in hostedServiceProperties.Elements()
                            select new
                            {
                                DeploymentStatus = (
                                from deployments in t.Descendants(wa + "Deployments")
                                select deployments.Element(wa + "Deployment").Element(wa + "Status").Value).First(),
                                RoleCount = (
                                from roles in t.Descendants(wa + "RoleList")
                                select roles.Elements()).Count(),
                                InstanceCount = (
                                from instances in t.Descendants(wa + "RoleInstanceList")
                                select instances.Elements()).Count()
                                
                            }).First();

            String deploymentStatus = deploymentInformation.DeploymentStatus;
            Int32 roleCount = deploymentInformation.RoleCount;
            Int32 instanceCount = deploymentInformation.InstanceCount;
        }


        public static void UseGetHostedServicePropertiesExample()
        {
            String subscriptionId = "SUBSCRIPTION_ID";
            String thumbprint = "THUMBPRINT";
            String serviceName = "SERVICE_NAME";
            GetHostedServicePropertiesExample example = new GetHostedServicePropertiesExample();

            example.GetHostedServiceProperties(subscriptionId, thumbprint, serviceName);
        }
    }
}
