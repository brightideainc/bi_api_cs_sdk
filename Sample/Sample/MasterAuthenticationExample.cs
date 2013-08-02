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
        Client client;

        public MasterAuthenticationExample()
        {
            client = new Client("MASTERKEY", "SECRET");

            Dictionary<string, object> tokens;
            tokens = client.AuthenticateMaster();

            Request request = new Request("member", ApiAction.INDEX);
            Dictionary<string, object> result = client.Execute(request).Deserialize<Dictionary<string, object>>();

            ArrayList memberList = (ArrayList)result["member_list"];
            Console.WriteLine(memberList.Count);

        }

    }
}
