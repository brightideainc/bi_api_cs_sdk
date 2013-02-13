using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BISDK
{
    public enum ApiAction
    {
        CREATE, LIST, UPDATE, INDEX, DELETE

    }
    public static class Extensions
    {
        public static Method ApiMethod(this ApiAction action)
        {
            Method method;
            switch (action)
            {
                case ApiAction.INDEX:
                case ApiAction.LIST:
                    method = Method.GET;
                    break;

                case ApiAction.CREATE:
                    method = Method.POST;
                    break;

                case ApiAction.DELETE:
                    method = Method.DELETE;
                    break;

                case ApiAction.UPDATE:
                    method = Method.PUT;
                    break;

                default:
                    method = Method.GET;
                    break;
            }
            return method;
        }
    }

    public class Request : RestRequest
    {

        public Request(string function, ApiAction action)
            : base("api3/"+function, action.ApiMethod())
        {

        }

        public Request(string function)
            : base("api3/" + function)
        {

        }
    }
}
