using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Configuration;
using RestSharp;

namespace AppstreamLib.RestCall
{

    public class RequestHandler
    {
        public static IRestResponse MakeGetRequest<T>(string token, string servicename , string tokenname = "token") where T : new()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["rest.call.endpoint"]))
                {
                    throw new Exception("Client address not specified");
                }

                var client = new RestClient(ConfigurationManager.AppSettings["rest.call.endpoint"] + servicename);
                var request = new RestRequest();

                request.Method = Method.GET;
                request.AddHeader("Content-type", "application/json");
                request.AddHeader(tokenname, token);

                var taskCompletionSource = new TaskCompletionSource<T>();
                var response = client.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.ErrorMessage);
                }
                else
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static IRestResponse MakePostRequest<T>(string token, string servicename, object body = null , string tokenname = "token") where T : new()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["rest.call.endpoint"]))
                {
                    throw new Exception("Client address not specified");
                }

                var client = new RestClient(ConfigurationManager.AppSettings["rest.call.endpoint"] + servicename);
                var request = new RestRequest();

                request.Method = Method.POST;
                request.AddHeader("Content-type", "application/json");
                request.AddHeader(tokenname, token);
                request.AddJsonBody(body);

                var taskCompletionSource = new TaskCompletionSource<T>();
                var response = client.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.ErrorMessage);
                }
                else
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public static object IdentityserverTokenRequest<T>(string endpoint, string body = "") where T : new()
        {
            try
            {
                var client = new RestClient(endpoint);
                var request = new RestRequest();

                request.Method = Method.POST;
                request.AddParameter("application/x-www-form-urlencoded", body , ParameterType.RequestBody);

                var taskCompletionSource = new TaskCompletionSource<T>();
                var response = client.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.ErrorMessage);
                }
                else
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
