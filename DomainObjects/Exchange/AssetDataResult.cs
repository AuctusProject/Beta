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
            [JsonProperty("ath")]
            public ValueResult AllTimeHigh { get; set; }
            [JsonProperty("ath_change_percentage")]
            public ValueResult AllTimeHighPercentage { get; set; }
            [JsonProperty("ath_date")]
            public DateResult AllTimeHighDate { get; set; }
            [JsonProperty("total_volume")]
            public ValueResult TotalVolume { get; set; }
            [JsonProperty("high_24h")]
            public ValueResult High24h { get; set; }
            [JsonProperty("low_24h")]
            public ValueResult Low24h { get; set; }
            [JsonProperty("market_cap_rank")]
            public int MarketCapRank { get; set; }
            [JsonProperty("total_supply")]
            public double? TotalSupply { get; set; }
            [JsonProperty("price_change_24h")]
            public double? PriceChange24h { get; set; }
            [JsonProperty("price_change_percentage_24h")]
            public double? PriceChangePercentage24h { get; set; }
            [JsonProperty("price_change_percentage_7d")]
            public double? PriceChangePercentage7d { get; set; }
            [JsonProperty("price_change_percentage_14d")]
            public double? PriceChangePercentage14d { get; set; }
            [JsonProperty("price_change_percentage_30d")]
            public double? PriceChangePercentage30d { get; set; }
            [JsonProperty("price_change_percentage_60d")]
            public double? PriceChangePercentage60d { get; set; }
            [JsonProperty("price_change_percentage_200d")]
            public double? PriceChangePercentage200d { get; set; }
            [JsonProperty("price_change_percentage_1y")]
            public double? PriceChangePercentage1y { get; set; }
            [JsonProperty("market_cap_change_24h")]
            public double? MarketCapChange24h { get; set; }
            [JsonProperty("market_cap_change_percentage_24h")]
            public double? MarketCapChangePercentage24h { get; set; }
            [JsonProperty("circulating_supply")]
            public double? CirculatingSupply { get; set; }
        }

        public class ValueResult
        {
            [JsonProperty("usd")]
            public double? Value { get; set; }
        }

        public class DateResult
        {
            [JsonProperty("usd")]
            public DateTime? Date { get; set; }
        }
    }
}
