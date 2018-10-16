using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class AssetRatingsResponse
    {
        public int AssetId { get; set; } 
        public int ExpertId { get; set; }
        public string ExpertName { get; set; }
        public double ExpertRating { get; set; }
        public DateTime? AdviceDate { get; set; }
        public int? AdviceType { get; set; }
        public double? AssetValue { get; set; }
    }
}
