using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class BinanceWebSocketTicker : BinanceTicker
    {
        [JsonProperty("e")]
        public string EventType { get; set; }
        [JsonProperty("E")]
        public double EventTime { get; set; }
        [JsonProperty("s")]
        public override string Symbol { get; set; }
        [JsonProperty("p")]
        public override double PriceChange { get; set; }
        [JsonProperty("P")]
        public override double PriceChangePercent { get; set; }
        [JsonProperty("w")]
        public override double WeightedAvgPrice { get; set; }
        [JsonProperty("x")]
        public override double PrevClosePrice { get; set; }
        [JsonProperty("c")]
        public override double LastPrice { get; set; }
        [JsonProperty("Q")]
        public override double LastQty { get; set; }
        [JsonProperty("b")]
        public override double BidPrice { get; set; }
        [JsonProperty("B")]
        public double BidQty { get; set; }
        [JsonProperty("a")]
        public override double AskPrice { get; set; }
        [JsonProperty("A")]
        public double AskQty { get; set; }
        [JsonProperty("o")]
        public override double OpenPrice { get; set; }
        [JsonProperty("h")]
        public override double HighPrice { get; set; }
        [JsonProperty("l")]
        public override double LowPrice { get; set; }
        [JsonProperty("v")]
        public override double Volume { get; set; }
        [JsonProperty("q")]
        public override double QuoteVolume { get; set; }
        [JsonProperty("O")]
        public override double OpenTime { get; set; }
        [JsonProperty("C")]
        public override double CloseTime { get; set; }
        [JsonProperty("F")]
        public override double FirstId { get; set; }
        [JsonProperty("L")]
        public override double LastId { get; set; }
        [JsonProperty("n")]
        public override double Count { get; set; }
    }
}
