if (typeof (TRIBRIDGEDEMO) == "undefined")
{ TRIBRIDGEDEMO = { __namespace: true }; }

;TRIBRIDGEDEMO = {
     name:'TRIBRIDGEDEMO',

     CreditLimit_OnChange: function() {
        var AccountId = Xrm.Page.data.entity.getId();;
        var AccountName = Xrm.Page.getAttribute("name").getValue();
        var requestString = "{'AccountName': '" + AccountName + "'," 
						    + "'AccountCRMId': '" + AccountId.replace("{","").replace("}","") + "'}";
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
                if (this.status == 200) {
                    var result = JSON.parse(this.response).data;
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
    }
}

//Function Calls
//TRIBRIDGEDEMO.CREDITLIMIT_ONCHANGE();