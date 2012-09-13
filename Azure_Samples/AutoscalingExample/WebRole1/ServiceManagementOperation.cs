using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Xml.Linq;
using System.IO;

namespace WebRole1
{
    public class ServiceManagementOperation
    {
        String thumbprint;
        String versionId = "2011-02-25";

        public ServiceManagementOperation(String thumbprint)
        {
            this.thumbprint = thumbprint;
        }

        private X509Certificate2 GetX509Certificate2(string thumbprint)
        {
            X509Certificate2 x509Certificate2 = null;
            X509Store store = new X509Store("My", StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection x509Certificate2Collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                x509Certificate2 = x509Certificate2Collection[0];
            }
            finally
            {
                store.Close();
            }
            return x509Certificate2;
        }

        private HttpWebRequest CreateHttpWebRequest(Uri uri, String httpWebRequestMethod)
        {
            X509Certificate2 x509Certificate2 = GetX509Certificate2(thumbprint);

            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            httpWebRequest.Method = httpWebRequestMethod;
            httpWebRequest.Headers.Add("x-ms-version", versionId);
            httpWebRequest.ClientCertificates.Add(x509Certificate2);
            httpWebRequest.ContentType = "application/xml";
            return httpWebRequest;
        }

        public XDocument Invoke(String uri)
        {
            XDocument responsePayload;
            Uri operationUri = new Uri(uri);
            HttpWebRequest httpWebRequest = CreateHttpWebRequest(operationUri, "GET");
            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                responsePayload = XDocument.Load(responseStream);
            }
            return responsePayload;
        }

        public String Invoke(String uri, XDocument payload)
        {
            Uri operationUri = new Uri(uri);
            HttpWebRequest httpWebRequest = CreateHttpWebRequest(operationUri, "POST");
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(requestStream, System.Text.UTF8Encoding.UTF8))
                {
                    payload.Save(streamWriter, SaveOptions.DisableFormatting);
                }
            }

            String requestId;
            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                requestId = response.Headers["x-ms-request-id"];
            }
            return requestId;
        }

    }
}