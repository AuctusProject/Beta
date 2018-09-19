using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Asset
{
    public class AssetCurrentValue : Asset
    {
        [DapperType(System.Data.DbType.Double)]
        public double CurrentValue { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? Last24HoursValue { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? Last7DaysValue { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? Last30DaysValue { get; set; }
    }
}
