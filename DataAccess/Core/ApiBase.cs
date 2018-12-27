using Auctus.Util;
using Auctus.Util.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Auctus.DataAccess.Core
{
    public abstract class ApiBase
    {
        protected string BearerToken { get; set; }
        protected string BaseAddress { get; private set; }

        protected ApiBase(string baseAddress)
        {
            BaseAddress = baseAddress;
        }

        protected string PostWithRetry(string route, object contentObject, params int[] consideredSuccessStatusCode)
        {
            return Retry.Get().Execute<string>((Func<string, object, int[], string>)Post, route, contentObject, consideredSuccessStatusCode);
        }

        protected string Post(string route, object contentObject, params int[] consideredSuccessStatusCode)
        {
            using (var client = CreateHttpClient())
            {
                var content = contentObject != null ? ParsePostContentObject(contentObject) : null;
                using (HttpResponseMessage response = client.PostAsync(route, content).Result)
                {
                    return HandleResponse(response, consideredSuccessStatusCode);
                }
            }
        }

        protected virtual StringContent ParsePostContentObject(object contentObject)
        {
            return new StringContent(JsonConvert.SerializeObject(contentObject), Encoding.UTF8, "application/json");
        }

        protected string GetWithRetry(string route, params int[] consideredSuccessStatusCode)
        {
            return Retry.Get().Execute<string>((Func<string, int[], string >)Get, route, consideredSuccessStatusCode);
        }

        protected string Get(string route, params int[] consideredSuccessStatusCode)
        {
            using (var client = CreateHttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(route).Result)
                {
                    return HandleResponse(response, consideredSuccessStatusCode);
                }
            }
        }

        protected virtual string HandleResponse(HttpResponseMessage response, params int[] consideredSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode || (consideredSuccessStatusCode != null && consideredSuccessStatusCode.Any() && consideredSuccessStatusCode.Contains((int)response.StatusCode)))
                return responseContent;
            else
                throw new ApiException((int)response.StatusCode, responseContent);
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseAddress);
            SetHttpClientHeaders(client);
            return client;
        }

        protected virtual void SetHttpClientHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if(!String.IsNullOrWhiteSpace(BearerToken))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
        }
    }
}
