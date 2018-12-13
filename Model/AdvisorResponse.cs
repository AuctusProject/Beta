using System;
using System.Collections.Generic;
using System.Text;
using static Auctus.Model.AssetResponse;

namespace Auctus.Model
{
    public class AdvisorResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UrlGuid { get; set; }
        public string Description { get; set; }
        public double SuccessRate { get; set; }
        public double AverageReturn { get; set; }
        public int Ranking { get; set; }
        public int TotalAdvisors { get; set; }
        public double Rating { get; set; }
        public int NumberOfFollowers { get; set; }
        public int TotalAssetsTraded { get; set; }
        public int TotalTrades { get; set; }
        public bool Following { get; set; }
        public bool Owner { get; set; }
        public DateTime CreationDate { get; set; }
        public double TotalAvailable { get; set; }
        public double TotalAllocated { get; set; }
        public double TotalBalance { get; set; }
        public double Equity { get; set; }
        public double TotalProfit { get; set; }
        public double TotalProfitPercentage { get; set; }
        public double? AverageTradeMinutes { get; set; }
        public DateTime? LastPortfolioReferenceDate { get; set; }
        public double? LastPortfolioValue { get; set; }
        public double? Profit24hValue { get; set; }
        public double? Profit24hPercentage { get; set; }
        public MonthlyRankingHistoryResponse MonthlyRankingHistory { get; set; }
        public List<AdvisorAssetHistoryResponse> AdvisorAsset24hHistory { get; set; } = new List<AdvisorAssetHistoryResponse>();
        public List<RecommendationDistributionResponse> RecommendationDistribution { get; set; } = new List<RecommendationDistributionResponse>();

        public List<AdvisorAssetResponse> OpenPositions { get; set; } = new List<AdvisorAssetResponse>();
        public List<AdvisorAssetResponse> ClosedPositions { get; set; } = new List<AdvisorAssetResponse>();

        public class AdvisorAssetResponse
        {
            public int AssetId { get; set; }
            public string AssetName { get; set; }
            public string AssetCode { get; set; }
            public PairResponse Pair { get; set; }
            public int Type { get; set; }
            public double SuccessRate { get; set; }
            public double AverageReturn { get; set; }
            public double AveragePrice { get; set; }
            public double TotalQuantity { get; set; }
            public double TotalInvested { get; set; }
            public double TotalProfit { get; set; }
            public double TotalVirtual { get; set; }
            public int OrderCount { get; set; }
            public int SuccessCount { get; set; }
            public int? SummedTradeMinutes { get; set; }
        }

        public class AdvisorAssetHistoryResponse
        {
            public int AssetId { get; set; }
            public double TotalQuantity { get; set; }
            public double TotalInvested { get; set; }
            public double TotalProfit { get; set; }
            public double TotalVirtual { get; set; }
            public double AveragePrice { get; set; }
            public double? Profit24hValue { get; set; }
            public double? Profit24hPercentage { get; set; }
        }

        public class MonthlyRankingHistoryResponse
        {
            public int Ranking { get; set; }
            public DateTime? PortfolioReferenceDate { get; set; }
            public double? PortfolioValue { get; set; }
            public double ProfitPercentage { get; set; }
        }
    }
}
