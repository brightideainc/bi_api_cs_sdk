using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BISDK
{
    public class Client : RestClient
    {
        protected string AppId;
        protected string AppSecret;
        protected string AccessToken;
        protected string RefreshToken;

        public Client(string domain, string appId, string appSecret) 
            : base("https://" + domain)
        {
            AppId = appId;
            AppSecret = appSecret;
        }

        public Client(string domain, string appId)
            : base("https://" + domain)
        {
            AppId = appId;
        }

        public Dictionary<string, object> Authenticate(string email, string password)
        {
            Request request = new Request("session", ApiAction.CREATE);
            request.AddParameter("app_id", AppId);
            if(email!=null)
                request.AddParameter("email", email);

            request.AddParameter("password", password);

            if(!String.IsNullOrEmpty(AppSecret))
                request.AddParameter("app_secret", AppSecret);

            Response response = Execute(request);
            Dictionary<string, object> responseDictionary = response.Deserialize<Dictionary<string, object>>();
            Dictionary<string, object> session = (Dictionary<string, object>)responseDictionary["session"];

            return session;
        }

        public Dictionary<string, object> Authenticate(string email)
        {
            return Authenticate(email, null);
        }

        public Dictionary<string, object> Authenticate()
        {
            return Authenticate(null,null);
        }

        public void AuthenticateWithAccessToken(string accessToken)
        {
            if (String.IsNullOrEmpty(accessToken))
                throw new InvalidAccessTokenException("Access token cannot be empty");
            AccessToken = accessToken;
        }

        public Dictionary<string, object> RenewAccessToken(string refreshToken)
        {
            Request request = new Request("session", ApiAction.CREATE);
            request.AddParameter("app_id", AppId);
            request.AddParameter("refresh_token", RefreshToken);

            Response response = Execute(request);
            Dictionary<string, object> responseDictionary = response.Deserialize<Dictionary<string, object>>();
            Dictionary<string, object> session = (Dictionary<string, object>)responseDictionary["session"];
            AccessToken = (string)session["access_token"];
            RefreshToken = (string)session["refresh_token"];

            return session;
        }

        public Response Execute(Request request)
        {
            ProcessRequest(request);

            var response = base.Execute(request);
            Response BIResponse = new Response(response.Content);

            PostResponse(BIResponse.JObject);

            return BIResponse;
        }

        public void ProcessRequest(Request req)
        {
            if (AccessToken != null)
            {
                req.AddHeader("ACCESS_TOKEN", AccessToken);
            }
        }

        public byte[] DownloadData(Request request)
        {
            ProcessRequest(request);

            byte[] response = base.DownloadData(request);
            return response;
        }

        protected IRestResponse DoExecute(Request req)
        {
            ProcessRequest(req);

            IRestResponse response = base.Execute(req);
            return response;
        }

        protected void PostResponse(JObject response)
        {
            if (response["errorCode"] != null)
            {
                ProcessError((int)response["errorCode"], (string)response["message"]);
            }
        }

        protected void ProcessError(int errorCode, string message)
        {
            switch (errorCode)
            {
                case 1001:
                    throw new InvalidRefreshTokenException(message);
                case 1002:
                    throw new InvalidAccessTokenException(message);
                case 13:
                    throw new MemberNotExistException(message);
                default:
                    throw new Exception(message);
            }
            
        }

    }
}
