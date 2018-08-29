using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Web3;
using Auctus.Util;
using Auctus.Util.Exceptions;
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
    public class Web3Api : ApiBase
    {
        private class AucAmountResponse
        {
            [JsonProperty("result")]
            public string Result { get; set; }
        }

        private readonly string BaseRoute;
        private readonly string AucContractAddress;

        public Web3Api(IConfigurationRoot configuration) : base(configuration.GetSection("Url:Web3").Get<string>())
        {
            BaseRoute = configuration.GetSection("Url:Web3Route").Get<string>(); 
            AucContractAddress = configuration.GetSection("AucContract").Get<string>(); 
        }

        public decimal GetAucAmount(string address)
        {
            address = address.ToLower().StartsWith("0x") ? address.Substring(2) : address;
            var response = GetWithRetry($"{BaseRoute}eth_call?params=[{{\"to\":\"{AucContractAddress}\",\"data\":\"0x70a08231000000000000000000000000{address}\"}},\"latest\"]");
            return Util.Util.ConvertHexaBigNumber(JsonConvert.DeserializeObject<AucAmountResponse>(response).Result, 18);
        }       
    }
}
