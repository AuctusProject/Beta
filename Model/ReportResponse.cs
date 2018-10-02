using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class ReportResponse
    {
        public int ReportId { get; set; }
        public DateTime ReportDate { get; set; }
        public int AssetId { get; set; }
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string AgencyWebSite { get; set; }
        public RatingDetail Rate { get; set; }
        public List<RatingDetail> RateOptions { get; set; }

        public class RatingDetail
        {
            public string Rate { get; set; }
            public string Description { get; set; }
            public string HexaColor { get; set; }
        }
    }
}
