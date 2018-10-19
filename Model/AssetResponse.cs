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
        public double LastValue { get; set; }
        public double? Variation24h { get; set; }
        public double? Variation7d { get; set; }
        public double? Variation30d { get; set; }
        public double? MarketCap { get; set; }
        public int? NumberOfFollowers { get; set; }
        public bool? Following { get; set; }
        public int Mode { get; set; }
        public List<RecommendationDistributionResponse> RecommendationDistribution { get; set; } = new List<RecommendationDistributionResponse>();
        public List<RecommendationDistributionResponse> ReportRecommendationDistribution { get; set; } = new List<RecommendationDistributionResponse>();

        public List<AssetAdvisorResponse> AssetAdvisor { get; set; } = new List<AssetAdvisorResponse>();
        public List<AdviceResponse> Advices { get; set; } = new List<AdviceResponse>();
        public List<ReportResponse> Reports { get; set; } = new List<ReportResponse>();
        public List<EventResponse> Events { get; set; } = new List<EventResponse>();

        public List<AdvisorResponse> Advisors { get; set; } = new List<AdvisorResponse>();

        public class AssetAdvisorResponse
        {
            public int UserId { get; set; }
            public double SuccessRate { get; set; }
            public double AverageReturn { get; set; }
            public double CurrentReturn { get; set; }
            public int TotalRatings { get; set; }
            public DateTime? LastAdviceDate { get; set; }
            public int? LastAdviceType { get; set; }
            public int? LastAdviceMode { get; set; }
            public double? LastAdviceAssetValue { get; set; }
            public double? LastAdviceTargetPrice { get; set; }
            public double? LastAdviceStopLoss { get; set; }
            public int? LastAdviceOperationType { get; set; }
            public List<AdviceResponse> Advices { get; set; } = new List<AdviceResponse>();
        }
        public class ValuesResponse
        {
            public DateTime Date { get; set; }
            public double Value { get; set; }
        }
        public class AdviceResponse
        {
            public int UserId { get; set; }
            public DateTime Date { get; set; }
            public int AdviceType { get; set; }
            public double AssetValue { get; set; }
            public double? TargetPrice { get; set; }
            public double? StopLoss { get; set; }
            public int OperationType { get; set; }
        }
    }
}
