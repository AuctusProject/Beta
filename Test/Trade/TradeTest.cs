using Auctus.DomainObjects.Trade;
using Auctus.Model;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Auctus.Test.Trade
{
    
    public class TradeTest : BaseTest
    {
        //private void TestIndividualOrder(int assetId, DomainObjects.Trade.OrderType type, double quantity, double? price = null, double? takeProfit = null)
        //{
        //    OrderBusiness.CreateOrder(assetId, type, quantity, price, takeProfit, null);
        //}

        //private void ValidatePosition(int assetId, DomainObjects.Trade.OrderType type, OrderStatusType orderStatus, double quantity, double? totalDollar = null)
        //{
        //    var positions = AdvisorProfitBusiness.ListAdvisorProfit(UserBusiness.GetLoggedUser().Id, new List<int>() { assetId });
        //    var assetPosition = positions.First(p => p.OrderType == type && p.OrderStatusType == orderStatus);
        //    Assert.Equal(quantity, assetPosition.TotalQuantity, 3);
        //    if(totalDollar.HasValue)
        //        Assert.Equal(totalDollar.Value, assetPosition.TotalDollar, 3);
        //}

        //[Fact]
        //public void TestCreateOrders()
        //{
        //    TestIndividualOrder(1, DomainObjects.Trade.OrderType.Buy, 1.3);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Executed, 1.3);
        //    TestIndividualOrder(1, DomainObjects.Trade.OrderType.Sell, 1.4);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Executed, 1.3);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Executed, 1.4);
        //    TestIndividualOrder(1, DomainObjects.Trade.OrderType.Buy, 0.7);
        //    TestIndividualOrder(1, DomainObjects.Trade.OrderType.Sell, 0.6);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Executed, 2);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Executed, 2);
        //    TestIndividualOrder(1, DomainObjects.Trade.OrderType.Buy, 0.7, 7400);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Executed, 2);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Open, 0.7);
        //    TestIndividualOrder(1, DomainObjects.Trade.OrderType.Sell, 1.1, 7500);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Executed, 2);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Open, 1.1);
        //    TestIndividualOrder(1, DomainObjects.Trade.OrderType.Sell, 0.5, 7600);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Executed, 2);
        //    ValidatePosition(1, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Open, 1.6);
        //}

        //[Fact]
        //public void TestTakeProfit()
        //{
        //    //595.28951907
        //    TestIndividualOrder(2, DomainObjects.Trade.OrderType.Buy, 1.3, null, 650);
        //    ValidatePosition(2, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Executed, 1.3, 1.3 * 595.28951907);
        //    ValidatePosition(2, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Open, 1.3, 1.3*650);
        //    var order = OrderBusiness.ListOrders(new List<int>() { UserBusiness.GetLoggedUser().Id }, new List<int>() { 2 }, new List<OrderStatusType>() { OrderStatusType.Open }).First();
        //    Assert.Equal(650.0, order.Price, 2);
        //    Assert.Equal(1.3, order.Quantity, 2);
        //}

        //[Fact]
        //public void TestClose()
        //{
        //    //0.667077627
        //    TestIndividualOrder(3, DomainObjects.Trade.OrderType.Buy, 1.5, null, 0.9);
        //    ValidatePosition(3, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Open, 1.5);
        //    var order = OrderBusiness.ListOrders(new List<int>() { UserBusiness.GetLoggedUser().Id }, new List<int>() { 3 }, new List<OrderStatusType>() { OrderStatusType.Executed }).First();
        //    OrderBusiness.CloseOrder(order.Id, 0.3);
        //    ValidatePosition(3, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Executed, 1.2);
        //    ValidatePosition(3, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Open, 1.2);
        //    var closeOrder = OrderBusiness.ListOrders(new List<int>() { UserBusiness.GetLoggedUser().Id }, new List<int>() { 3 }, new List<OrderStatusType>() { OrderStatusType.Close }).First();
        //    Assert.Equal(order.Id, closeOrder.OrderId);
        //    Assert.NotEqual(order.OrderType, closeOrder.OrderType);
        //    Assert.Equal(OrderStatusType.Close, closeOrder.OrderStatusType);

        //    OrderBusiness.CloseOrder(order.Id, 0.5);
        //    ValidatePosition(3, DomainObjects.Trade.OrderType.Buy, OrderStatusType.Executed, 0.7);
        //    ValidatePosition(3, DomainObjects.Trade.OrderType.Sell, OrderStatusType.Open, 0.7);

        //    Assert.Throws<BusinessException>(() => OrderBusiness.CloseOrder(order.Id, 0.8)); 
            
        //}
    }
}

