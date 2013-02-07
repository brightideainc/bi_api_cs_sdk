using BISDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class SSOAuthenticationExample
    {
        string RefreshToken;
        string AccessToken;
        Client client;

        public void SSOAuthenticationExample()
        {
            client = new Client("ziqi.brightideadev.com", "MASTERKEY", "SECRET");
            string sampleUserEmail = "zjin@brightidea.com";

            Dictionary<string, object> tokens;
            try
            {
                tokens = client.Authenticate(sampleUserEmail);

            }
            catch (MemberNotExistException ex)
            {
                //Create user on the fly with master account
                Client masterClient = new Client("ziqi.brightideadev.com", "MASTERKEY", "SECRET");
                masterClient.Authenticate();
                Request request = new Request("member",ApiAction.CREATE);
                request.AddParameter("screen_name", "Sample User");
                request.AddParameter("email", sampleUserEmail);
                Dictionary<string, object> userCreationResult = masterClient.Execute(request);

                //if auto user creation failed, handle it here.

                //try authenticate again
                tokens = client.Authenticate(sampleUserEmail);
            }

            string accessToken = (string)tokens["access_token"];

            try
            {
                client.AuthenticateWithAccessToken(AccessToken);

                Request request = new Request("member", ApiAction.INDEX);
                Dictionary<string, object> result = Execute(request);

                ArrayList memberList = (ArrayList)result["member_list"];
                Console.WriteLine(memberList.Count);
            }
            catch (Exception ex)
            {
                //server error
            }
        }

        public Dictionary<string, object> Execute(Request request)
        {
            return Execute(request, false);
        }

        public Dictionary<string, object> Execute(Request request, bool retry)
        {
            Dictionary<string, object> result = null;
            try
            {
                result = client.Execute(request);
            }
            catch (InvalidAccessTokenException ex)
            {
                RefreshAccessToken();
                if (!retry)
                    result = Execute(request, true);
            }

            return result;
        }

        public void RefreshAccessToken()
        {
            Dictionary<string, object> tokens = client.Authenticate("zjin@brightidea.com");
            string accessToken = (string)tokens["access_token"];
        }
    }
}
