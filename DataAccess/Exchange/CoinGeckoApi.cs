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
        private const string MARKETCHART_ROUTE = "api/v3/coins/{0}/market_chart?vs_currency=usd&days={1}";
        private const string LASTDATA_ROUTE = "api/v3/coins/{0}?localization=false&sparkline=false";

        public CoinGeckoApi() : base("https://api.coingecko.com") { }

        public IEnumerable<AssetResult> GetAllCoinsData()
        {
            var responseContent = GetWithRetry(COINDATA_ROUTE);
            return JsonConvert.DeserializeObject<AssetResult[]>(responseContent);
        }

        public AssetPricesResult GetAssetPrices(string assetId, int days)
        {
            if (string.IsNullOrEmpty(assetId) || days <= 0)
                return null;

            var responseContent = GetWithRetry(String.Format(MARKETCHART_ROUTE, assetId, days));
            return JsonConvert.DeserializeObject<AssetPricesResult>(responseContent);
        }

        public AssetDataResult GetCoinData(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
                return null;

            var responseContent = GetWithRetry(String.Format(LASTDATA_ROUTE, assetId));
            return JsonConvert.DeserializeObject<AssetDataResult>(responseContent);
        }
    }
}
