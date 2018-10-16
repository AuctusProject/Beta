using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Model.Advisor
{
    public class AdviseRequest
    {
        public int AssetId { get; set; }
        public int AdviceType { get; set; }
        public double? StopLoss { get; set; }
        public double? TargetPrice { get; set; }
    }
}
