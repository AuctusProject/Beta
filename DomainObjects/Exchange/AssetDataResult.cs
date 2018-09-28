using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class AssetDataResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("image")]
        public ImageResult Image { get; set; }
        [JsonProperty("market_data")]
        public MarketDataResult MarketData { get; set; }

        public class ImageResult
        {
            [JsonProperty("large")]
            public string ImageUrl { get; set; }
        }

        public class MarketDataResult
        {
            [JsonProperty("current_price")]
            public ValueResult CurrentPrice { get; set; }
            [JsonProperty("market_cap")]
            public ValueResult MarketCap { get; set; }
        }

        public class ValueResult
        {
            [JsonProperty("usd")]
            public double? Value { get; set; }
        }
    }
}
