using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class AssetPricesResult
    {
        [JsonProperty("prices")]
        public double[][] Prices { get; set; }
    }
}
