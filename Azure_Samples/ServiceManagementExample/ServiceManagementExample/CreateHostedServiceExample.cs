using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

namespace ServiceManagementExample
{
    class CreateHostedServiceExample
    {
        XNamespace wa = "http://schemas.microsoft.com/windowsazure";
        String createHostedServiceFormat = "https://management.core.windows.net/{0}/services/hostedservices";

        private String ConvertToBase64String(String value)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            String base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        private XDocument CreatePayload(String serviceName, String label, String description, String location, String affinityGroup)
        {
            String base64LabelName = ConvertToBase64String(label);
            
            XElement xServiceName = new XElement(wa + "ServiceName", serviceName);
            XElement xLabel = new XElement(wa + "Label", base64LabelName);
            XElement xDescription = new XElement(wa + "Description", description);
            XElement xLocation = new XElement(wa + "Location", location);
            XElement xAffinityGroup = new XElement(wa + "AffinityGroup", affinityGroup);
            XElement createHostedService = new XElement(wa +"CreateHostedService");

            createHostedService.Add(xServiceName);
            createHostedService.Add(xLabel);
            createHostedService.Add(xDescription);
            createHostedService.Add(xLocation);
            //createHostedService.Add(xAffinityGroup);
            XDocument payload = new XDocument();
            payload.Add(createHostedService);
            payload.Declaration = new XDeclaration("1.0", "UTF-8", "no");

            return payload;
        }

        private String CreateHostedService(String subscriptionId, String thumbprint, String serviceName, String label, String description, String location, String affinityGroup)
        {
            String uri = String.Format(createHostedServiceFormat, subscriptionId);

            XDocument payload = CreatePayload(serviceName, label, description, location, affinityGroup);
            ServiceManagementOperation operation = new ServiceManagementOperation(thumbprint);
            String requestId = operation.Invoke(uri, payload);
            return requestId;
        }

        public static void UseCreateHostedServiceExample()
        {
            String subscriptionId = "SUBSCRIPTION_ID";
            String thumbprint = "THUMBPRINT";
            String serviceName = "SERVICE_NAME";
            String label = "LABEL";
            String description = "DESCRIPTION";
            String location = "North Central US";
            String affinityGroup = "AFFINITY_GROUP_GUID";
            CreateHostedServiceExample example = new CreateHostedServiceExample();
            String requestId = example.CreateHostedService( subscriptionId, thumbprint, serviceName, label, description, location, affinityGroup);
        }
    }
}
