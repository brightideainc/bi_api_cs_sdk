using BISDK;
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
        string RefreshToken;
        string AccessToken;
        Client client;

        public AuthenticationExample()
        {
            client = new Client("ziqi.brightideadev.com", "APIKEY");

            Dictionary<string, object> tokens = client.Authenticate("zjin@brightidea.com", "1234qwer");
            string accessToken = (string)tokens["access_token"];
            string refreshToken = (string)tokens["refresh_token"];

            try
            {
                client.AuthenticateWithAccessToken(AccessToken);

                Request request = new Request("member", ApiAction.INDEX);
                Dictionary<string, object> result = Execute(request);

                ArrayList memberList = (ArrayList)result["member_list"];
                Console.WriteLine(memberList.Count);
            }
            catch (InvalidUserCridentialException ex)
            {
                //prompt for email password (Reason, wrong email/password)
            }
            catch (InvalidRefreshTokenException ex)
            {
                //prompt for email password (Reason, reauthentication required)            
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
                RefreshAccessToken(RefreshToken);
                if (!retry)
                    result = Execute(request, true);
            }

            return result;
        }

        public void RefreshAccessToken(string refreshToken)
        {
            Dictionary<string, object> tokens = client.RenewAccessToken(refreshToken);
            AccessToken = (string)tokens["access_token"];
            RefreshToken = (string)tokens["refresh_token"];
        }
    }
}
