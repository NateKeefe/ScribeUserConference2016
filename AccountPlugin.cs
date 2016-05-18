using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Xml;

namespace ScribeUserConference.CRM2016.Plugins
{
    public class AccountPlugin : IPlugin
    {
        string scribeURL = string.Empty;
        public AccountPlugin(string unsecureConfig, string secureConfig)
        {
            XmlDocument unsecureDOC = new XmlDocument();
            if (unsecureConfig == null)
            {
                unsecureConfig = "<Settings>" +
                                 "<setting name='ScribeURL'>" +
                                 "   <value>https://endpoint.scribesoft.com/v1/orgs/16551/requests/1741?accesstoken=40834938-1e6c-4fd1-9b9c-d358e7f21f5c</value>" +
                                 " </setting>" +
                                 "</Settings>";
            }
            unsecureDOC.LoadXml(unsecureConfig);
            scribeURL = PluginConfiguration.GetConfigDataString(unsecureDOC, "ScribeURL");
        }
        public void Execute(IServiceProvider serviceProvider)
        {

            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            // Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            // Use the factory to generate the Organization Service.
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && ((context.InputParameters["Target"] is Entity)))
            {
                var currentEntity = (Entity)context.InputParameters["Target"];
                string pre_accountname = "";
                decimal pre_accountcreditlimit = 0.0M;


                if (context.PreEntityImages.Contains("Account-PreImage") && (context.PreEntityImages["Account-PreImage"] is Entity))
                {
                    Entity preImageEntity = (Entity)context.PreEntityImages["Account-PreImage"];

                    pre_accountname = preImageEntity.Attributes.Contains("name") && preImageEntity.Attributes["name"] != null ? preImageEntity.GetAttributeValue<string>("name").ToString() : "";
                    pre_accountcreditlimit = decimal.Parse((preImageEntity.Attributes.Contains("creditlimit") && preImageEntity.Attributes["creditlimit"] != null) ? preImageEntity.GetAttributeValue<Money>("creditlimit").Value.ToString() : "0.00");
                }
                    //This part of the query is only for updating status
                if (context.MessageName.Contains("Update") && currentEntity.LogicalName == "account")
                {
                    //1. Get the agent GUID
                    string accountGuid = currentEntity.Id.ToString();
                    //2. Get the new values
                    string new_accountname = currentEntity.Attributes.Contains("name") && currentEntity.Attributes["name"] != null ? currentEntity.GetAttributeValue<string>("name").ToString() : "";
                    decimal new_accountcreditlimit = 0.0M;
                    new_accountcreditlimit = decimal.Parse((currentEntity.Attributes.Contains("creditlimit") && currentEntity.Attributes["creditlimit"] != null) ? currentEntity.GetAttributeValue<Money>("creditlimit").Value.ToString() : "0.00");

                    if ((pre_accountname != new_accountname) || (pre_accountcreditlimit != new_accountcreditlimit))   
                    {
                        
                        var requestString = "{"
                                               + "\"CRMAccountName\" : \"" + pre_accountname + "\","
                                               + "\"CRMAccountCreditLimit\" : \"" + new_accountcreditlimit.ToString() + "\""
                                            + " }";

                        tracingService.Trace(requestString.ToString());
                        bool result = AppCode.SendAccountsToLegacySystem(requestString, scribeURL);
                    }
                    
                }
                ////Update of the fields from Create
                if (context.MessageName.Contains("Create") && currentEntity.LogicalName == "account")
                {
                    //1. Get the Account GUID
                    string accountGuid = currentEntity.Id.ToString();
                    //2. Get the values
                    string new_accountname = currentEntity.Attributes.Contains("name") && currentEntity.Attributes["name"] != null ? currentEntity.GetAttributeValue<string>("name").ToString() : "";
                    decimal new_accountcreditlimit = decimal.Parse(currentEntity.Attributes.Contains("creditlimit") && currentEntity.Attributes["creditlimit"] != null ? currentEntity.GetAttributeValue<Money>("creditlimit").Value.ToString() : "0.00");
                    var requestString = "{"
                                             + "\"CRMAccountName\" : \"" + new_accountname + "\","
                                             + "\"CRMAccountCreditLimit\" : \"" + new_accountcreditlimit.ToString() + "\""
                                          + " }";

                    tracingService.Trace(requestString.ToString());
                    bool result = AppCode.SendAccountsToLegacySystem(requestString, scribeURL);

                }
            }

        }//End of Execute
    }
}
