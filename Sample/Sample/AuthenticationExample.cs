using BISDK;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class AuthenticationExample
    {
        Client client;

        public AuthenticationExample()
        {
            string ClientId = "ClientID";
            string ClientSecret = "SECRET";
            string Email = "user@brightidea.com";
            string Password = "password";

            //Create the client
            client = new Client(ClientId, ClientSecret);
            //client.CustomDomain="auth.brightideasandbox.com";

            //Authenticate with user's email and password
            Dictionary<string, object> tokens = client.Authenticate(Email, Password);

            //Make API request to pull the member list
            Request request = new Request("member", ApiAction.INDEX);
            Response response = client.Execute(request);

            /////////////////////////////////////////
            // There are 3 ways to read the response
            /////////////////////////////////////////

            //1.Read data by using JSON.Net
            JObject json = response.JObject;
            JArray memberList = (JArray)json["member_list"];

            System.Diagnostics.Debug.WriteLine(memberList.Count);

            //2.Read data by deserializing the json
            Dictionary<string, object> dictionary = response.Deserialize<Dictionary<string, object>>();

            //3.Get the content as string
            string content = response.Content;

        }

    }
}
