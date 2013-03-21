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
        protected string _accessToken;
        public string AccessToken { 
            get {
                if(String.IsNullOrEmpty(_accessToken) && PersistentDataManager != null)
                {
                    _accessToken = PersistentDataManager.GetPersistentData("access_token");
                }
                return _accessToken;
            }
            set {
                _accessToken = value;
                if (PersistentDataManager != null)
                {
                    PersistentDataManager.SetPersistentData("access_token", _accessToken);
                }
            }
        }

        protected string _refreshToken;
        protected string RefreshToken {
            get
            {
                if (String.IsNullOrEmpty(_refreshToken) && PersistentDataManager != null)
                {
                    _refreshToken = PersistentDataManager.GetPersistentData("refresh_token");
                }
                return _refreshToken;
            }
            set
            {
                _refreshToken = value;
                if (PersistentDataManager != null)
                {
                    PersistentDataManager.SetPersistentData("refresh_token", _refreshToken);
                }
            }
        }
        public string Email;
        public PersistentDataManager PersistentDataManager;
        public const string DefaultDomain = "api3.brightidea.com";

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
            if (!String.IsNullOrEmpty(email))
                request.AddParameter("email", email);

            if (!String.IsNullOrEmpty(password))
            request.AddParameter("password", password);

            //clear old access token
            AccessToken = null;

            if(!String.IsNullOrEmpty(AppSecret))
                request.AddParameter("app_secret", AppSecret);

            Response response = Execute(request);
            Dictionary<string, object> responseDictionary = response.Deserialize<Dictionary<string, object>>();
            Dictionary<string, object> session = (Dictionary<string, object>)responseDictionary["session"];

            AccessToken = (string)session["access_token"];
            if (session.ContainsKey("refresh_token"))
            {
                RefreshToken = (string)session["refresh_token"];
            }

            return session;
        }

        public Dictionary<string, object> Authenticate(string email)
        {
            Email = email;
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
            return Execute(request, 0);
        }

        protected Response Execute(Request request, int retry)
        {
            Response response = null;
            try
            {
                response = DoExecute(request);
            }
            catch (InvalidAccessTokenException ex)
            {
                
                if (retry < 1)
                {
                    RefreshAccessToken();
                    response = Execute(request, 1);
                }
                    
            }

            return response;
        }

        protected void RefreshAccessToken()
        {
            if (!String.IsNullOrEmpty(AppSecret))
            {
                //for master authentication, just authenticate again to get the new access token
                Dictionary<string, object> tokens;
                if (!String.IsNullOrEmpty(Email))
                    tokens = Authenticate(Email);
                else
                    tokens = Authenticate();

                AccessToken = (string)tokens["access_token"];
            }
            else
            {
                //for regular authentication, use refresh token to renew access token
                RenewAccessToken(RefreshToken);
            }
            
        }

        protected Response DoExecute(Request request)
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
                case 1004:
                    throw new InvalidRefreshTokenException(message);
                case 1002:
                    throw new InvalidAccessTokenException(message);
                case 1003:
                    throw new MemberNotExistException(message);
                default:
                    throw new Exception(message);
            }
            
        }

    }
}
