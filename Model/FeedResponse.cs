using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class FeedResponse
    {
        public DateTime Date { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetCode { get; set; }
        public int AssetMode { get; set; }
        public bool FollowingAsset { get; set; }
        public ReportResponse Report { get; set; }
        public AdviceResponse Advice { get; set; }
        public EventResponse Event { get; set; }

        public class AdviceResponse
        {
            public int AdvisorId { get; set; }
            public string AdvisorName { get; set; }
            public string AdvisorUrlGuid { get; set; }
            public int AdvisorRanking { get; set; }
            public double AdvisorRating { get; set; }
            public bool FollowingAdvisor { get; set; }
            public int AdviceId { get; set; }
            public int AdviceType { get; set; }
            public double AssetValueAtAdviceTime { get; set; }
            public double? TargetPrice { get; set; }
            public double? StopLoss { get; set; }
            public int OperationType { get; set; }
        }
    }
}
