using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class FeedResponse
    {
        public int AdviceId { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetCode { get; set; }
        public int AdvisorId { get; set; }
        public string AdvisorName { get; set; }
        public string AdvisorUrlGuid { get; set; }
        public int AdvisorRanking { get; set; }
        public bool FollowingAdvisor { get; set; }
        public bool FollowingAsset{ get; set; }
        public int AdviceType { get; set; }
        public DateTime Date { get; set; }        
    }
}
