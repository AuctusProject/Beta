using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class BinanceTicker
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("priceChange")]
        public double PriceChange { get; set; }
        [JsonProperty("priceChangePercent")]
        public double PriceChangePercent { get; set; }
        [JsonProperty("weightedAvgPrice")]
        public double WeightedAvgPrice { get; set; }
        [JsonProperty("prevClosePrice")]
        public double PrevClosePrice { get; set; }
        [JsonProperty("lastPrice")]
        public double LastPrice { get; set; }
        [JsonProperty("lastQty")]
        public double LastQty { get; set; }
        [JsonProperty("bidPrice")]
        public double BidPrice { get; set; }
        [JsonProperty("askPrice")]
        public double AskPrice { get; set; }
        [JsonProperty("openPrice")]
        public double OpenPrice { get; set; }
        [JsonProperty("highPrice")]
        public double HighPrice { get; set; }
        [JsonProperty("lowPrice")]
        public double LowPrice { get; set; }
        [JsonProperty("volume")]
        public double Volume { get; set; }
        [JsonProperty("quoteVolume")]
        public double QuoteVolume { get; set; }
        [JsonProperty("openTime")]
        public double OpenTime { get; set; }
        [JsonProperty("closeTime")]
        public double CloseTime { get; set; }
        [JsonProperty("firstId")]
        public double FirstId { get; set; }
        [JsonProperty("lastId")]
        public double LastId { get; set; }
        [JsonProperty("count")]
        public double Count { get; set; }
    }
}
