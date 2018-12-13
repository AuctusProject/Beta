using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Trade
{
    public class Order
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int UserId { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int AssetId { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int Type { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int Status { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime StatusDate { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double Quantity { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double Price { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? TakeProfit { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? StopLoss { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double? Profit { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int? OrderId { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int ActionType { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime? OpenDate { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double RemainingQuantity { get; set; }

        public OrderType OrderType { get { return OrderType.Get(Type); } }
        public OrderActionType OrderActionType { get { return OrderActionType.Get(ActionType); } }
        public OrderStatusType OrderStatusType { get { return OrderStatusType.Get(Status); } }
        public List<Order> RelatedOrders { get; set; } = new List<Order>();
    }
}
