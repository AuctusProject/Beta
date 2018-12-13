using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class AdvisorPerformanceResponse
    {
        public double DailyDrawdown { get; set; }
        public double BestTrade { get; set; }
        public double WorstTrade { get; set; }
        public List<DailyPerformanceResponse> DailyPerformance { get; set; } = new List<DailyPerformanceResponse>();

        public class DailyPerformanceResponse
        {
            public DateTime Date { get; set; }
            public double Equity { get; set; }
            public double Variation { get; set; }
        }
    }
}
