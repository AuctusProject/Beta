using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Asset
{
    public class AssetCurrentValue : Asset
    {
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime UpdateDate { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double CurrentValue { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? Variation24Hours { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? Variation7Days { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? Variation30Days { get; set; }
    }
}
