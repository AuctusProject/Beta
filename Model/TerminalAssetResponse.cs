using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class TerminalAssetResponse
    {
        public int AssetId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ChartPair { get; set; }
        public string ChartExchange { get; set; }
    }
}
