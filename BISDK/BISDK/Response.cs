using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace BISDK
{
    public class Response
    {
        public JObject JObject;
        public string Content;

        public Response(string content)
        {
            Content = content;
            JObject = GetJObject();
        }

        private JObject GetJObject()
        {
            JObject result = JObject.Parse(Content);
            return result;
        }

        public T Deserialize<T>() 
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            T deserializedDictionary = ser.Deserialize<T>(Content);
            return deserializedDictionary;
        }
    }

}
