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
        Client client;

        public SSOAuthenticationExample()
        {
            client = new Client("MASTERKEY", "SECRET");
            string sampleUserEmail = "user@brightidea.com";

            Dictionary<string, object> tokens=null;
            try
            {
                tokens = client.AuthenticateMaster(sampleUserEmail);

            }
            catch (MemberNotExistException ex)
            {
                //Create user on the fly with master account
                Client masterClient = new Client("MASTERKEY", "SECRET");
                masterClient.AuthenticateMaster();
                Request request = new Request("member", ApiAction.CREATE);
                request.AddParameter("screen_name", "Sample User");
                request.AddParameter("email", sampleUserEmail);
                Dictionary<string, object> userCreationResult = masterClient.Execute(request).Deserialize<Dictionary<string, object>>();

                //if auto user creation failed, handle it here.

                //try authenticate again
                tokens = client.AuthenticateMaster(sampleUserEmail);
            }


            Request request2 = new Request("member", ApiAction.INDEX);
            Dictionary<string, object> result = client.Execute(request2).Deserialize<Dictionary<string, object>>();

            ArrayList memberList = (ArrayList)result["member_list"];


        }

    }
}
