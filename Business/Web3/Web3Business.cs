using Auctus.DomainObjects.Web3;
using Auctus.Util;
using Auctus.Util.NotShared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Auctus.Business.Web3
{
    public class Web3Business
    {
        private const string BASE_ROUTE = "v1/jsonrpc/mainnet/";

        public static decimal GetAucAmount(string address)
        {
            address = address.ToLower().StartsWith("0x") ? address.Substring(2) : address;
            var response = GetWithRetry(string.Format("eth_call?params=[{{\"to\":\"0xc12d099be31567add4e4e4d0d45691c3f58f5663\",\"data\":\"0x70a08231000000000000000000000000{0}\"}},\"latest\"]", address));
            return Util.Util.ConvertHexaBigNumber(response.ToString(), 18);
        }

        private static object PostWithRetry(string route, object contentObject)
        {
            return Retry.Get().Execute<object>((Func<string, object, object >)Post, route, contentObject);
        }

        private static object Post(string route, object contentObject)
        {
            using (var client = CreateWeb3Client())
            {
                var content = contentObject != null ? new StringContent(JsonConvert.SerializeObject(contentObject), Encoding.UTF8, "application/json") : null;
                using (HttpResponseMessage response = client.PostAsync(BASE_ROUTE + route, content).Result)
                {
                    return HandleResponse(response);
                }
            }
        }

        private static object GetWithRetry(string route)
        {
            return Retry.Get().Execute<object>((Func<string, object>)Get, route);
        }

        private static object Get(string route)
        {
            using (var client = CreateWeb3Client())
            {
                using (HttpResponseMessage response = client.GetAsync(BASE_ROUTE + route).Result)
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

        private static HttpClient CreateWeb3Client()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Config.WEB3_URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
