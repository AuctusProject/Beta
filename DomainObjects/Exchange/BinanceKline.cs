using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class BinanceKline
    {
        public double OpenTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close{ get; set; }
        public double Volume { get; set; }
        public double CloseTime { get; set; }
    }
}
