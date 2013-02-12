using BISDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class MasterAuthenticationExample
    {
        string RefreshToken;
        string AccessToken;
        Client client;

        public MasterAuthenticationExample()
        {
            client = new Client("ziqi.brightideadev.com", "MASTERKEY", "SECRET");

            Dictionary<string, object> tokens;
            tokens = client.Authenticate();

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
