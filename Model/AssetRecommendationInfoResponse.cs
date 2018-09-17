using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class AssetRecommendationInfoResponse
    {
        public int AssetId { get; set; }
        public double LastValue { get; set; }
        public bool CloseRecommendationEnabled { get; set; }
    }
}
