using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class AssetResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("image")]
        public string ImageUrl { get; set; }
        [JsonProperty("current_price")]
        public double? Price { get; set; }
        [JsonProperty("market_cap")]
        public double? MarketCap { get; set; }
        [JsonProperty("market_cap_rank")]
        public int? MarketCapRank { get; set; }
        [JsonProperty("total_volume")]
        public double? TotalVolume { get; set; }
        [JsonProperty("high_24h")]
        public double? High24h { get; set; }
        [JsonProperty("low_24h")]
        public double? Low24h { get; set; }
        [JsonProperty("price_change_24h")]
        public double? Variation24h { get; set; }
        [JsonProperty("price_change_percentage_24h")]
        public double? VariationPercentage24h { get; set; }
        [JsonProperty("market_cap_change_24h")]
        public double? MarketCapVariation24h { get; set; }
        [JsonProperty("market_cap_change_percentage_24h")]
        public double? MarketCapPercentage24h { get; set; }
        [JsonProperty("circulating_supply")]
        public double? CirculatingSupply { get; set; }
        [JsonProperty("total_supply")]
        public double? TotalSupply { get; set; }
        [JsonProperty("ath")]
        public double? AllTimeHigh { get; set; }
        [JsonProperty("ath_change_percentage")]
        public double? AllTimeHighPercentage { get; set; }
        [JsonProperty("ath_date")]
        public DateTime? AllTimeHighDate { get; set; }
    }
}
