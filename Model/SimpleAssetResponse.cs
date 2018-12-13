using System;
using System.Collections.Generic;
using System.Text;
using static Auctus.Model.AssetResponse;

namespace Auctus.Model
{
    public class SimpleAssetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Type { get; set; }
        public bool ShortSellingEnabled { get; set; }
        public double? MarketCap { get; set; }
        public double? CirculatingSupply { get; set; }
        public PairResponse Pair { get; set; }

    }
}
