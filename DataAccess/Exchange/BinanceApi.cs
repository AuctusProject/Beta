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
        private const string KLINE_7D = "api/v1/klines?symbol={0}&interval=1w&limit=1";
        private const string KLINE_30D = "api/v1/klines?symbol={0}&interval=1M&limit=1";

        public BinanceApi() : base("https://api.binance.com") { }

        public BinanceTicker[] GetTicker24h()
        {
            var responseContent = GetWithRetry(TICKER_24H);
            return JsonConvert.DeserializeObject<BinanceTicker[]>(responseContent);
        }

        public BinanceTicker GetTicker24h(string symbol)
        {
            var responseContent = GetWithRetry(TICKER_24H + $"?symbol={symbol.ToUpper()}");
            return JsonConvert.DeserializeObject<BinanceTicker>(responseContent);
        }

        public BinanceKline GetKline30d(string symbol)
        {
            var responseContent = GetWithRetry(String.Format(KLINE_7D, symbol));
            return ConvertResponseToBinanceKline(responseContent);
        }

        public BinanceKline GetKline7d(string symbol)
        {
            var responseContent = GetWithRetry(String.Format(KLINE_30D, symbol));
            return ConvertResponseToBinanceKline(responseContent);
        }

        private BinanceKline ConvertResponseToBinanceKline(string responseContent)
        {
            var klines = JsonConvert.DeserializeObject<List<double[]>>(responseContent);
            if (klines == null || klines.Count == 0)
            {
                return null;
            }

            var kline = klines[0];
            return new BinanceKline()
            {
                OpenTime = kline[0],
                Open = kline[1],
                High = kline[2],
                Low = kline[3],
                Close = kline[4],
                Volume = kline[5],
                CloseTime = kline[6],
            };
        }
    }
}
