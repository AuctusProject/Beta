using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class AssetResponse
    {
        public int AssetId { get; set; } 
        public string Name { get; set; }
        public string Code { get; set; }
        public int TotalRatings { get; set; }
        public int TotalAdvisors { get; set; }
        public int Mode { get; set; }
        public double LastValue { get; set; }
        public bool ShortSellingEnabled { get; set; }
        public double? Variation24h { get; set; }
        public double? Variation7d { get; set; }
        public double? Variation30d { get; set; }
        public double? MarketCap { get; set; }
        public double? CirculatingSupply { get; set; }
        public int? NumberOfFollowers { get; set; }
        public bool? Following { get; set; }
        public PairResponse Pair { get; set; }
        public List<RecommendationDistributionResponse> RecommendationDistribution { get; set; } = new List<RecommendationDistributionResponse>();
        public List<RecommendationDistributionResponse> ReportRecommendationDistribution { get; set; } = new List<RecommendationDistributionResponse>();
        
        public class ValuesResponse
        {
            public DateTime Date { get; set; }
            public double Value { get; set; }
        }
        public class PairResponse
        {
            public string Symbol { get; set; }
            public string MultipliedSymbol { get; set; }
            public string Preffix { get; set; }
            public string Suffix { get; set; }
        }
    }
}
