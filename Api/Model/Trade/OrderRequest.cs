using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Model.Trade
{
    public class OrderRequest
    {
        public int AssetId { get; set; }
        public int Type { get; set; }
        public double Quantity { get; set; }
        public double? Price { get; set; }
        public double? TakeProfit { get; set; }
        public double? StopLoss { get; set; }
    }
}
