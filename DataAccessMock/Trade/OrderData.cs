using Auctus.DataAccessInterfaces.Trade;
using Auctus.DomainObjects.Trade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.DataAccessMock.Trade
{
    public class OrderData : BaseData<Order>, IOrderData<Order>
    {
        private static List<Order> orders = new List<Order>()
        {
        };
        
        public List<Order> ListOrders(IEnumerable<int> usersId, IEnumerable<int> assetsId, IEnumerable<OrderStatusType> ordersStatusType, OrderType orderType)
        {
            return orders.Where(o =>
                (usersId == null || !usersId.Any() || usersId.Contains(o.UserId)) &&
                (assetsId == null || !assetsId.Any() || assetsId.Contains(o.AssetId)) &&
                (ordersStatusType == null || !ordersStatusType.Any() || ordersStatusType.Contains(o.OrderStatusType)) &&
                (orderType == null || orderType == o.OrderType)).ToList();
        }

        public List<Order> ListOrdersForRankingProfitCalculation(IEnumerable<int> usersId, IEnumerable<int> assetsId)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Order obj)
        {
            orders.Remove(obj);
        }

        public override void Insert(Order obj)
        {
            if(obj.Id == 0)
            {
                obj.Id = orders.Any() ? orders.Max(o => o.Id)+1 : 1;
            }
            orders.Add(obj);
        }

        public override void Update(Order obj)
        {
            //Do nothing
        }

        public Order Get(int orderId)
        {
            return orders.FirstOrDefault(o => o.Id == orderId);
        }

        public List<Order> ListRelatedOrders(IEnumerable<int> ordersId)
        {
            return orders.Where(o => ordersId.Any(c => c == o.OrderId)).ToList();
        }

        public List<Order> ListOpenOrdersAndExecutedWithStopLoss(IEnumerable<int> assetsIds)
        {
            return orders.Where(o =>
                (assetsIds == null || !assetsIds.Any() || assetsIds.Contains(o.AssetId)) &&
                (o.OrderStatusType == OrderStatusType.Open ||
                (o.OrderStatusType == OrderStatusType.Executed && o.StopLoss.HasValue))).ToList();
        }

        public List<Order> ListLastAdvisorsOrdersForAsset(int assetId, IEnumerable<OrderStatusType> orderStatusTypes)
        {
            throw new NotImplementedException();
        }

        public List<Order> ListOrdersWithRelated(IEnumerable<int> usersId, IEnumerable<int> assetsId, IEnumerable<OrderStatusType> ordersStatusType, OrderType orderType)
        {
            throw new NotImplementedException();
        }

        public Order GetWithRelated(int orderId)
        {
            throw new NotImplementedException();
        }

        public Order GetAnyOrderCreatedByUser(int userId, int usdAssetId)
        {
            throw new NotImplementedException();
        }
    }
}
