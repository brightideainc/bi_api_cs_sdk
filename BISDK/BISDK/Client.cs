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
        protected string ClientId;
        protected string ClientSecret;
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
                this.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_accessToken);
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
        public const string DefaultDomain = "https://auth.brightidea.com";
        public bool MasterAuthentication = false;
        protected string _customDomain;
        public string CustomDomain
        {
            get
            {
                return _customDomain;
            }
            set
            {
                this.BaseUrl = "https://" + value;
                _customDomain = value;
            }
        }

        public Client(string clientId, string clientSecret) 
            : base(DefaultDomain)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public Response Authenticate(string email, string password)
        {
            RestRequest request = new RestRequest("_oauth2/token", Method.POST);
            request.AddParameter("client_id", ClientId);
            if (!String.IsNullOrEmpty(email))
            {
                Email = email;
                request.AddParameter("username", email);
            }
                

            if (!String.IsNullOrEmpty(password))
                request.AddParameter("password", password);

            request.AddParameter("grant_type", "password");

            //clear old access token, we don't want access token in this request
            AccessToken = null;
            this.Authenticator = null;

            if (String.IsNullOrEmpty(ClientSecret))
                throw new Exception("Client secret cannot be blank");

            request.AddParameter("client_secret", ClientSecret);

            Response response = Execute(request);
            JObject responseObject = response.JObject;

            AccessToken = (string)responseObject["access_token"];


            if (responseObject["refresh_token"]!=null)
            {
                RefreshToken = (string)responseObject["refresh_token"];
            }

            if (CustomDomain==null && responseObject["systems"] != null)
            {
                JArray Systems = (JArray)responseObject["systems"];
                if (Systems.Count > 0)
                {
                    this.BaseUrl = "https://" + Systems[0]["host_name"];
                }
            }
            
            

            return response;
        }

        public Response AuthenticateMaster(string email)
        {
            MasterAuthentication = true;
            
            RestRequest request = new RestRequest("_oauth2/token", Method.POST);
            request.AddParameter("client_id", ClientId);
            if (!String.IsNullOrEmpty(email))
            {
                Email = email;
                request.AddParameter("email", email);
            }

            request.AddParameter("grant_type", "client_credentials");

            //clear old access token, we don't want access token in this request
            AccessToken = null;
            this.Authenticator = null;

            if (!String.IsNullOrEmpty(ClientSecret))
                request.AddParameter("client_secret", ClientSecret);

            Response response = Execute(request);
            JObject responseObject = response.JObject;

            AccessToken = (string)responseObject["access_token"];

            return response;
        }

        public Response AuthenticateMaster()
        {
            return AuthenticateMaster(null);
        }

        public Dictionary<string, object> RenewAccessToken(string refreshToken)
        {
            RestRequest request = new RestRequest("_oauth2/token", Method.POST);
            request.AddParameter("client_id", ClientId);
            request.AddParameter("client_secret", ClientSecret);
            request.AddParameter("refresh_token", RefreshToken);
            request.AddParameter("grant_type", "refresh_token");

            Response response = Execute(request);
            Dictionary<string, object> responseDictionary = response.Deserialize<Dictionary<string, object>>();
            AccessToken = (string)responseDictionary["access_token"];
            
            RefreshToken = (string)responseDictionary["refresh_token"];

            return responseDictionary;
        }

        public Response Execute(RestRequest request)
        {
            return Execute(request, 0);
        }

        protected Response Execute(RestRequest request, int retry)
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
            if (MasterAuthentication)
            {
                //for master authentication, just authenticate again to get the new access token
                AuthenticateMaster(Email);
            }
            else
            {
                //for regular authentication, use refresh token to renew access token
                RenewAccessToken(RefreshToken);
            }
            
        }

        protected Response DoExecute(RestRequest request)
        {
            ProcessRequest(request);

            var response = base.Execute(request);

            if (response.ErrorException != null)
                throw response.ErrorException;

            Response BIResponse = new Response(response.Content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && (string)BIResponse.JObject["error"] == "invalid_grant")
            {
                 throw new InvalidAccessTokenException("Invalid access token");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception((string)BIResponse.JObject["error_description"]);
            }

            if (BIResponse.JObject["errorCode"] != null)
            {
                ProcessError((int)BIResponse.JObject["errorCode"], (string)BIResponse.JObject["message"]);
            }

            return BIResponse;
        }

        public void ProcessRequest(RestRequest req)
        {
            if (AccessToken == null)
            {
                req.AddParameter("client_id", ClientId);
            }
        }

        public byte[] DownloadData(Request request)
        {
            ProcessRequest(request);

            byte[] response = base.DownloadData(request);
            return response;
        }

        protected void ProcessError(int errorCode, string message)
        {
            switch (errorCode)
            {
                case 1003:
                    throw new MemberNotExistException(message);
                default:
                    throw new Exception(message);
            }
            
        }

        public bool LoadPersistentData()
        {
            String accessToken = AccessToken;
            String refreshToken = RefreshToken;

            if (String.IsNullOrEmpty(accessToken))
            {
                return false;
            }
            else
            {
                this.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken);
            }

            return true;
        }

    }
}
