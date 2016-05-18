using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.IO;
using System.Net;

namespace ScribeUserConference.CRM2016.Plugins
{
    public static class AppCode
    {
        public static bool SendAccountsToLegacySystem(string requestString, string scribeURL)
        {
            bool result = false;
            //scribeURL = "https://endpoint.scribesoft.com/v1/orgs/14514/requests/1556?accesstoken=4c168b86-31a2-4732-bb4a-d097c7077963";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(scribeURL);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(requestString);
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    if (responseText.Contains("Success"))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

    }
}

