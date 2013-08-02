C# SDK for the new API
=============

## Get Started:
    * Instructions

## Basic Usage:

    //Create the client
    client = new Client(ClientId, ClientSecret);
    
    //Authenticate with user's email and password
    Dictionary<string, object> tokens = client.Authenticate(Email, Password);
    
    //Make API request to pull the member list
    Request request = new Request("member", ApiAction.INDEX);
    Response response = client.Execute(request);
    
    JObject json = response.JObject;
    JArray memberList = (JArray)json["member_list"];
    int memberCount = memberList.Count
    
