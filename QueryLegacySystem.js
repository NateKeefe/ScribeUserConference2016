if (typeof (TRIBRIDGEDEMO) == "undefined")
{ TRIBRIDGEDEMO = { __namespace: true }; }

;TRIBRIDGEDEMO = {
     name:'TRIBRIDGEDEMO',

     CreditLimit_OnChange: function() {
        //alert("On Change Fired");
        var AccountId = Xrm.Page.data.entity.getId();;
        var AccountName = Xrm.Page.getAttribute("name").getValue();
        //alert(AccountName);
   
        var requestString = "{'AccountName': '" + AccountName + "'," 
						    + "'AccountCRMId': '" + AccountId.replace("{","").replace("}","") + "'}";
        //alert(requestString);
        TRIBRIDGEDEMO.QueryLegacySystem(requestString);
     }
    ,
    QueryLegacySystem: function(requestString) {
        var req = new XMLHttpRequest()
        req.open("POST",encodeURI("https://endpoint.scribesoft.com/v1/orgs/16551/requests/1740?accesstoken=40834938-1e6c-4fd1-9b9c-d358e7f21f5c"), true);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState == 4 /* complete */) {
                req.onreadystatechange = null;
                //alert(this.status);
                if (this.status == 200) {
                   // alert("success");
                    //Parse the Result and populate the Legacy System Data on to the CRM Form
                    /*
                        {"data": [{
                            "legacyAccountId": 1,
                            "legacyCreditLimit": 10000,
                            "accountGuid": "b90a213d-dabf-4757-97dc-42f13a0ac164",
                            "accountName": "Contoso"
                        }]} 
                    */
                    var result = JSON.parse(this.response).data;
                    //alert(result[0].legacyCreditLimit);
                    Xrm.Page.getAttribute("new_legacyid").setValue(result[0].legacyAccountId.toString());
                    Xrm.Page.getAttribute("new_legacycreditlimit").setValue(result[0].legacyCreditLimit.toString());
                }
                else {
                    var error = JSON.parse(this.response).error;
                    alert("Error-->" + error.message);
                }
            }
        };
        req.send(requestString);
        //req.send(JSON.stringify(requestString));
        //req.send(JSON.stringify({
        //	   "AgentId": "123455959",
        //	   "AgentName": "Agent Name1",
        //	   "ServiceName": "MoneyOrder",
        //	   "ServiceActivatedDeactivated": true,
        //	   "ActivationDate": "1997-07-16",
        //	   "DeActivationDate": "1997-07-16"
        //	}));
    }
    ,
    GetClientUrl: function() {
        if (typeof Xrm.Page.context == "object") {
            clientUrl = Xrm.Page.context.getClientUrl();
        }
        var ServicePath = "/XRMServices/2011/Organization.svc/web";
        return clientUrl + ServicePath;
    }
    ,
    convertToJson: function(text) {
        return JSON.parse(text);
    }
}

//Function Calls
//TRIBRIDGEDEMO.CREDITLIMIT_ONCHANGE();