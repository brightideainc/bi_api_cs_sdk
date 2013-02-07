using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

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

            if(AppSecret!=null)
                request.AddParameter("app_secret", AppSecret);

            return this.Execute(request);
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
            AccessToken = accessToken;
        }

        public Dictionary<string, object> RenewAccessToken(string refreshToken)
        {
            Request request = new Request("session", ApiAction.CREATE);
            request.AddParameter("app_id", AppId);
            request.AddParameter("refresh_token", RefreshToken);

            Dictionary<string, object> tokens = this.Execute(request);
            AccessToken = (string)tokens["access_token"];
            RefreshToken = (string)tokens["refresh_token"];

            return tokens;
        }

        public Dictionary<string, object> Execute(Request req)
        {
            var res = DoExecute(req);
            Dictionary<string, object> result = DeserializeString(res.Content);
            PostResponse(result);
            return result;
        }

        protected Dictionary<string, object> DeserializeString(string str)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            Dictionary<string, object> deserializedDictionary = ser.Deserialize<Dictionary<string, object>>(str);
            return deserializedDictionary;
        }

        public string ExecuteString(Request req)
        {
            var res = DoExecute(req);
            Dictionary<string, object> result = DeserializeString(res.Content);
            PostResponse(result);
            return res.Content;
        }

        public void ProcessRequest(Request req)
        {
            if (AccessToken != null)
            {
                req.AddHeader("ACCESS_TOKEN", AccessToken);
            }
        }

        public byte[] DownloadData(Request req)
        {
            ProcessRequest(req);

            byte[] response = base.DownloadData(req);
            return response;
        }

        protected IRestResponse DoExecute(Request req)
        {
            ProcessRequest(req);

            IRestResponse response = base.Execute(req);
            return response;
        }

        protected void PostResponse(Dictionary<string, object> response)
        {
            if (response.ContainsKey("errorCode"))
            {
                switch ((int)response["errorCode"])
                { 
                    case 1001:
                        throw new InvalidRefreshTokenException();
                    case 1002:
                        throw new InvalidAccessTokenException();
                    case 1003:
                        throw new MemberNotExistException();
                    default:
                        throw new Exception((string)response["error"]);
                }
            }
        }

    }
}
