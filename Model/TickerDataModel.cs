using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class TickerDataModel
    {
        public double CurrentValue { get; set; }
        public double? Variation24Hours { get; set; }
        public double BidValue { get; set; }
        public double AskValue { get; set; }
    }
}
