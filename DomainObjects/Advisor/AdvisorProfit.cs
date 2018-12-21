using Auctus.DomainObjects.Trade;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class AdvisorProfit
    {
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int UserId { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int AssetId { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int Status { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int Type { get; set; }
        [DapperCriticalRestrictionAttribute(DapperCriticalRestrictionAttribute.Operation.PreviousValueIsLesserOrEqual)]
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime UpdateDate { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double SummedProfitPercentage { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double SummedProfitDollar { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double TotalDollar { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double TotalQuantity { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int OrderCount { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int SuccessCount { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int? SummedTradeMinutes { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? TotalFee { get; set; }

        public OrderStatusType OrderStatusType { get { return OrderStatusType.Get(Status); } }
        public OrderType OrderType { get { return OrderType.Get(Type); } }
    }
}
