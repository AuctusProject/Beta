using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class AdvisorResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double SuccessRate { get; set; }
        public double AverageReturn { get; set; }
        public int Ranking { get; set; }
        public double Rating { get; set; }
        public int NumberOfFollowers { get; set; }
        public int TotalAssetsAdvised { get; set; }
        public bool Following { get; set; }
        public bool Owner { get; set; }
        public DateTime CreationDate { get; set; }
        public List<RecommendationDistributionResponse> RecommendationDistribution { get; set; } = new List<RecommendationDistributionResponse>();

        public List<AssetResponse> Assets { get; set; } = new List<AssetResponse>();
    }
}
