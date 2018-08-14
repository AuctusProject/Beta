using Auctus.DomainObjects.Web3;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Auctus.DataAccess.Blockchain
{
    public class Web3Api
    {
        private readonly string Web3Url;
        private readonly string BaseRoute;
        private readonly string AucContractAddress;

        public Web3Api(IConfigurationRoot configuration)
        {
            Web3Url = configuration.GetSection("Url:Web3").Get<string>();
            BaseRoute = configuration.GetSection("Url:Web3Route").Get<string>(); 
            AucContractAddress = configuration.GetSection("AucContract").Get<string>(); 
        }

        public decimal GetAucAmount(string address)
        {
            address = address.ToLower().StartsWith("0x") ? address.Substring(2) : address;
            var response = GetWithRetry($"eth_call?params=[{{\"to\":\"{AucContractAddress}\",\"data\":\"0x70a08231000000000000000000000000{address}\"}},\"latest\"]");
            return Util.Util.ConvertHexaBigNumber(response.ToString(), 18);
        }

        private object PostWithRetry(string route, object contentObject)
        {
            return Retry.Get().Execute<object>((Func<string, object, object>)Post, route, contentObject);
        }

        private object Post(string route, object contentObject)
        {
            using (var client = CreateWeb3Client())
            {
                var content = contentObject != null ? new StringContent(JsonConvert.SerializeObject(contentObject), Encoding.UTF8, "application/json") : null;
                using (HttpResponseMessage response = client.PostAsync(BaseRoute + route, content).Result)
                {
                    return HandleResponse(response);
                }
            }
        }

        private object GetWithRetry(string route)
        {
            return Retry.Get().Execute<object>((Func<string, object>)Get, route);
        }

        private object Get(string route)
        {
            using (var client = CreateWeb3Client())
            {
                using (HttpResponseMessage response = client.GetAsync(BaseRoute + route).Result)
                {
                    return HandleResponse(response);
                }
            }
        }

        private static object HandleResponse(HttpResponseMessage response)
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                var infuraResponse = JsonConvert.DeserializeObject<InfuraResponse>(responseContent);
                if (infuraResponse.Error != null)
                    throw new Web3Exception(infuraResponse.Error.Code ?? 400, infuraResponse.Error.Message);
                else
                    return infuraResponse.Result;
            }
            else
                throw new Exception(responseContent);
        }

        private HttpClient CreateWeb3Client()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Web3Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
