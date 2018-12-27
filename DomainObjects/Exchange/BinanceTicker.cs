using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class BinanceTicker
    {
        [JsonProperty("symbol")]
        public virtual string Symbol { get; set; }
        [JsonProperty("priceChange")]
        public virtual double PriceChange { get; set; }
        [JsonProperty("priceChangePercent")]
        public virtual double PriceChangePercent { get; set; }
        [JsonProperty("weightedAvgPrice")]
        public virtual double WeightedAvgPrice { get; set; }
        [JsonProperty("prevClosePrice")]
        public virtual double PrevClosePrice { get; set; }
        [JsonProperty("lastPrice")]
        public virtual double LastPrice { get; set; }
        [JsonProperty("lastQty")]
        public virtual double LastQty { get; set; }
        [JsonProperty("bidPrice")]
        public virtual double BidPrice { get; set; }
        [JsonProperty("askPrice")]
        public virtual double AskPrice { get; set; }
        [JsonProperty("openPrice")]
        public virtual double OpenPrice { get; set; }
        [JsonProperty("highPrice")]
        public virtual double HighPrice { get; set; }
        [JsonProperty("lowPrice")]
        public virtual double LowPrice { get; set; }
        [JsonProperty("volume")]
        public virtual double Volume { get; set; }
        [JsonProperty("quoteVolume")]
        public virtual double QuoteVolume { get; set; }
        [JsonProperty("openTime")]
        public virtual double OpenTime { get; set; }
        [JsonProperty("closeTime")]
        public virtual double CloseTime { get; set; }
        [JsonProperty("firstId")]
        public virtual double FirstId { get; set; }
        [JsonProperty("lastId")]
        public virtual double LastId { get; set; }
        [JsonProperty("count")]
        public virtual double Count { get; set; }
    }
}
