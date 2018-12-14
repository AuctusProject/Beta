using Auctus.DomainObjects.Trade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Auctus.DataAccessInterfaces.Trade
{
    public interface IOrderData<Order> : IBaseData<Order>
    {
        List<Order> ListOrders(IEnumerable<int> usersId, IEnumerable<int> assetsId, IEnumerable<OrderStatusType> ordersStatusType, OrderType orderType);
        List<Order> ListOrdersForRankingProfitCalculation(IEnumerable<int> usersId, IEnumerable<int> assetsId);
        Order Get(int orderId);
        Order GetWithRelated(int orderId);
        List<Order> ListRelatedOrders(IEnumerable<int> ordersId);
        List<Order> ListOpenOrdersAndExecutedWithStopLoss(IEnumerable<int> assetsIds);
        List<Order> ListLastAdvisorsOrdersForAsset(int assetId, IEnumerable<OrderStatusType> orderStatusTypes);
        List<Order> ListOrdersWithRelated(IEnumerable<int> usersId, IEnumerable<int> assetsId, IEnumerable<OrderStatusType> ordersStatusType, OrderType orderType);
        Order GetAnyOrderCreatedByUser(int userId, int usdAssetId);
        List<Order> ListUsersOrdersByDate(DateTime startStatusDate, IEnumerable<OrderStatusType> orderStatusTypes, IEnumerable<int> usersId, int top, int usdAssetId);
        IEnumerable<int> ListTrendingAssetIdsBasedOnOrders(int[] orderStatusList, int limit = 10, int numberOfDays = 7);
    }
}
