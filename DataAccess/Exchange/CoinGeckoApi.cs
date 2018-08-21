using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Auctus.DataAccess.Exchange
{
    public class CoinGeckoApi : ApiBase, ICoinGeckoApi
    {
        private const string FULLDATA_ROUTE = "api/v3/coins?order=gecko_desc&localization=false&per_page=100&page=";
        private const string LISTING_ROUTE = "api/v3/coins/list";
        private const string COINDATA_ROUTE = "api/v3/coins/markets?vs_currency=usd&order=gecko_desc";

        public CoinGeckoApi() : base("https://api.coingecko.com") { }

        public IEnumerable<AssetResult> GetAllCoinsData()
        {
            var responseContent = GetWithRetry(COINDATA_ROUTE);
            return JsonConvert.DeserializeObject<AssetResult[]>(responseContent);
        }
    }
}
