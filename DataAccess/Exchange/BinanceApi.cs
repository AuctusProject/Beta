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
        private const string TICKER_24H = "api/v1/ticker/24hr";

        public BinanceApi() : base("https://api.binance.com") { }

        
        public BinanceTicker[] GetTicker24h()
        {
            var responseContent = GetWithRetry(TICKER_24H);
            return JsonConvert.DeserializeObject<BinanceTicker[]>(responseContent);
        }
    }
}
