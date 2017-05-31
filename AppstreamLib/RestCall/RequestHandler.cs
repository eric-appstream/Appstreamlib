﻿using System;
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
        public static async Task<object> MakeGetRequest<T>(string token, string servicename , string tokenname = "token" , bool isIdsrvr = false ) 
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (tokenname == "Authorization")
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                }
                else
                {
                    client.DefaultRequestHeaders.Add(tokenname, token);
                }

                var httpresponse = await client.GetAsync(ConfigurationManager.AppSettings["rest.call.endpoint"] + servicename);
                var jsonContent = await httpresponse.Content.ReadAsStringAsync();

                if (httpresponse.StatusCode == HttpStatusCode.Forbidden)
                {

                    throw new Exception(httpresponse.ReasonPhrase);
                }
                else if (httpresponse.StatusCode != HttpStatusCode.OK)
                {
                    var responsestatus = JsonConvert.DeserializeObject<ResponseStatusRoot>(jsonContent);
                    throw new Exception(responsestatus.ResponseStatus.Message);
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<T>(jsonContent);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }   


        public static async Task<object> MakePostRequest<T>(string token, string servicename, object body = null , string tokenname = "token") 
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["rest.call.endpoint"]))
                {
                    throw new Exception("Client address not specified");
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if(tokenname == "Authorization")
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                }
                else
                {
                    client.DefaultRequestHeaders.Add(tokenname, token);
                }

                string postbody = JsonConvert.SerializeObject(body);

                var httpresponse = await client.PostAsync(ConfigurationManager.AppSettings["rest.call.endpoint"] + servicename, new StringContent(postbody, Encoding.UTF8, "text/json"));
                var jsonContent =  await httpresponse.Content.ReadAsStringAsync();

                if (httpresponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception(httpresponse.ReasonPhrase);
                }
                else if (httpresponse.StatusCode != HttpStatusCode.OK && httpresponse.StatusCode != HttpStatusCode.Created)
                {
                    var responsestatus = JsonConvert.DeserializeObject<ResponseStatusRoot>(jsonContent);
                    throw new Exception(responsestatus.ResponseStatus.Message);
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<T>(jsonContent);
                    return result;
                }

                /*
                var client = new RestClient(ConfigurationManager.AppSettings["rest.call.endpoint"] + servicename);
                var request = new RestRequest();

                request.Method = Method.POST;
                request.AddHeader("Content-type", "application/json");
                request.AddHeader(tokenname, token);

                if(tokenname == "Authorization")
                {
                    request.AddParameter(tokenname, token ,ParameterType.HttpHeader);
                }

                request.AddJsonBody(body);

                var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();

                client.ExecuteAsync(request, response => {

                    taskCompletionSource.SetResult(response);
                });

                return taskCompletionSource.Task;

                //var response = client.Execute<T>(request);
                /*
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.ErrorMessage + response.Content);
                }
                else
                {
                    return response;
                }*/
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

    public class ResponseStatusRoot
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ResponseStatus
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
