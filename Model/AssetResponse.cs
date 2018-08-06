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
        public double Variation24h { get; set; }
        public double Variation7d { get; set; }
        public double Variation30d { get; set; }
        public int? NumberOfFollowers { get; set; }
        public bool? Following { get; set; }
        public int Mode { get; set; }
        public List<RecommendationDistributionResponse> RecommendationDistribution { get; set; } = new List<RecommendationDistributionResponse>();
        public List<ValuesResponse> Values { get; set; } = new List<ValuesResponse>();

        public List<AssetAdvisorResponse> AssetAdvisor { get; set; } = new List<AssetAdvisorResponse>();
        public List<AdviceResponse> Advices { get; set; } = new List<AdviceResponse>();

        public List<AdvisorResponse> Advisors { get; set; } = new List<AdvisorResponse>();

        public class AssetAdvisorResponse
        {
            public int UserId { get; set; }
            public double SuccessRate { get; set; }
            public double AverageReturn { get; set; }
            public int TotalRatings { get; set; }
            public DateTime LastAdviceDate { get; set; }
            public int LastAdviceType { get; set; }
            public int LastAdviceMode { get; set; }
            public List<AdviceResponse> Advices { get; set; } = new List<AdviceResponse>();
        }
        public class ValuesResponse
        {
            public DateTime Date { get; set; }
            public double Value { get; set; }
        }
        public class AdviceResponse
        {
            public DateTime Date { get; set; }
            public int AdviceType { get; set; }
        }
    }
}
