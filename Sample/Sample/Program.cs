using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BISDK;
using RestSharp;
using System.Collections;

namespace Sample
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Program p = new Program();

            //Normal authentication
            //AuthenticationExample auth = new AuthenticationExample();

            //Master key authentication
            //MasterAuthenticationExample master = new MasterAuthenticationExample();

            //SSO Master key authentication
            SSOAuthenticationExample sso = new SSOAuthenticationExample();
        }

        

    }
}
