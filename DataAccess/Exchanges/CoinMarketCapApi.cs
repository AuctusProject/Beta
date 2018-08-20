using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Auctus.Util;
using Auctus.DomainObjects.Portfolio;
using Auctus.Util.Exceptions;
using Auctus.DataAccess.Core;
using System.Net;
using Auctus.DomainObjects.Exchange;

namespace Auctus.DataAccess.Exchanges
{
    public class CoinMarketCapApi : ApiBase
    {
        private const string COINMARKETCAP_ICONS_BASE_URL = @"https://s2.coinmarketcap.com/static/img/coins/32x32/";
        private const string FULLDATA_ROUTE = "v2/ticker/?limit=100&sort=id&structure=array&start=";
        private const string LISTING_ROUTE = "v2/listings/"; 

        private CoinMarketCapApi() : base("https://api.coinmarketcap.com/") { }

        public static CoinMarketCapApi Instance => new CoinMarketCapApi();

        public IEnumerable<AssetResult> GetAllCoinsData()
        {
            var currentPage = 0;
            var dictionary = new Dictionary<string, dynamic>();
            while (true)
            {
                var responseContent = GetWithRetry($"{FULLDATA_ROUTE}{(currentPage * 100 + 1)}", (int)HttpStatusCode.NotFound);
                var result = JsonConvert.DeserializeObject<CoinMarketCapResult>(responseContent);
                if (result != null && result.Data != null && result.Data.Any(c => c.Quotes?.USD != null))
                {
                    var data = result.Data.Where(c => c.Quotes?.USD != null)
                        .Select(c => new KeyValuePair<string, dynamic>(c.Id.ToString(), 
                            new
                            {
                                c.Name,
                                c.Symbol,
                                c.Quotes.USD.Price,
                                c.Quotes.USD.MarketCap
                            }));
                    dictionary = dictionary.Concat(data).ToDictionary(c => c.Key, c => c.Value);
                }
                else
                    break;

                currentPage++;
            }

            return dictionary.Select(c => new AssetResult()
            {
                Id = c.Key,
                Name = c.Value.Name,
                Symbol = c.Value.Symbol,
                Price = c.Value.Price,
                MarketCap = c.Value.MarketCap,
                ImageUrl = $"{COINMARKETCAP_ICONS_BASE_URL}{c.Key}.png"
            });
        }

        private class CoinMarketCapResult
        {
            [JsonProperty("data")]
            public CoinMarketCapDataResult[] Data { get; set; }
        }

        private class CoinMarketCapDataResult
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("symbol")]
            public string Symbol { get; set; }
            [JsonProperty("quotes")]
            public CoinMarketCapQuoteResult Quotes{ get; set; }
        }

        private class CoinMarketCapQuoteResult
        {
            [JsonProperty("USD")]
            public CoinMarketCapDollarResult USD { get; set; }
        }

        private class CoinMarketCapDollarResult
        {
            [JsonProperty("price")]
            public double? Price { get; set; }
            [JsonProperty("market_cap")]
            public double? MarketCap { get; set; }
        }
    }
}
