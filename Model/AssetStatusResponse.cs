using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class AssetStatusResponse
    {
        public int AssetId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        public double? MarketCap { get; set; }
        public int? MarketCapRank { get; set; }
        public double? TotalVolume { get; set; }
        public double? High24h { get; set; }
        public double? Low24h { get; set; }
        public double? Variation24h { get; set; }
        public double? VariationPercentage24h { get; set; }
        public double? MarketCapVariation24h { get; set; }
        public double? MarketCapPercentage24h { get; set; }
        public double? CirculatingSupply { get; set; }
        public double? TotalSupply { get; set; }
        public double? AllTimeHigh { get; set; }
        public double? AllTimeHighPercentage { get; set; }
        public DateTime? AllTimeHighDate { get; set; }
    }
}
