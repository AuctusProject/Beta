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
    public class BinanceApi : ApiBase, IBinanceApi
    {
        private const string EXCHANGE_INFO = "api/v1/exchangeInfo";
        private const string TICKER_24H = "api/v1/ticker/24hr";

        public BinanceApi() : base("https://api.binance.com") { }

        public ExchangeInfo GetCurrentTicker()
        {
            var responseContent = GetWithRetry(EXCHANGE_INFO);
            return JsonConvert.DeserializeObject<ExchangeInfo>(responseContent);
        }


        public class ExchangeInfo
        {
            [JsonProperty("symbols")]
            public SymbolInfo[] Symbols { get; set; }
        }

        public class SymbolInfo
        {
            [JsonProperty("symbol")]
            public String Symbol { get; set; }
            [JsonProperty("status")]
            public String Status { get; set; }
            [JsonProperty("baseAsset")]
            public String BaseAsset { get; set; }
            [JsonProperty("quoteAsset")]
            public String QuoteAsset { get; set; }
            [JsonProperty("orderTypes")]
            public String[] OrderTypes { get; set; }
        }
    }
}
