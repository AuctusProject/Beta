using Auctus.DataAccessInterfaces.Trade;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Trade;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.Business.Trade
{
    public class OrderBusiness : BaseBusiness<Order, IOrderData<Order>>
    {
        public OrderBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public OrderResponse EditStopLoss(int orderId, double? price)
        {
            var loggedUser = GetValidUser();
            var order = Data.GetWithRelated(orderId);
            BaseValidationOnPositionUpdate(order, loggedUser);

            var asset = AssetBusiness.GetById(order.AssetId);
            if (!asset.Enabled)
                throw new BusinessException("Invalid asset.");

            if (order.OrderType == OrderType.Sell)
            {
                var forcedStopLoss = order.Price * 2;
                if (!price.HasValue)
                    price = forcedStopLoss;
                if (price > forcedStopLoss)
                    throw new BusinessException($"The maximum stop loss value is {Util.Util.GetFormattedValue(forcedStopLoss)}.");
            }
            if (price.HasValue)
            {
                var currentAssetValue = AssetCurrentValueBusiness.GetRealCurrentValue(order.AssetId);
                if ((price.Value <= 0) || (order.OrderType == OrderType.Buy && currentAssetValue.BidValue <= price.Value) || (order.OrderType == OrderType.Sell && currentAssetValue.AskValue >= price.Value))
                    throw new BusinessException($"Invalid stop loss value for current price {Util.Util.GetFormattedValue(order.OrderType == OrderType.Buy ? currentAssetValue.BidValue : currentAssetValue.AskValue)}.");
            }
            
            order.StopLoss = price;
            Data.Update(order);
            return GetOrderResponse(loggedUser, order, AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id), new List<DomainObjects.Asset.Asset>() { asset });
        }

        public OrderResponse EditTakeProfit(int orderId, double? price)
        {
            var loggedUser = GetValidUser();
            var order = Data.GetWithRelated(orderId);
            BaseValidationOnPositionUpdate(order, loggedUser);

            var asset = AssetBusiness.GetById(order.AssetId);
            if (!asset.Enabled)
                throw new BusinessException("Invalid asset.");

            if (price.HasValue)
            {
                var currentAssetValue = AssetCurrentValueBusiness.GetRealCurrentValue(order.AssetId);
                if ((price.Value <= 0) || (order.OrderType == OrderType.Buy && currentAssetValue.BidValue >= price.Value) || (order.OrderType == OrderType.Sell && currentAssetValue.AskValue <= price.Value))
                    throw new BusinessException($"Invalid take profit value for current price {Util.Util.GetFormattedValue(order.OrderType == OrderType.Buy ? currentAssetValue.BidValue : currentAssetValue.AskValue)}.");
            }
            var now = Data.GetDateTimeNow();
            var oldTakeProfitOrder = order.RelatedOrders.FirstOrDefault(c => c.OrderStatusType == OrderStatusType.Open);
            order.TakeProfit = price;
            var newTakeProfitOrder = CreateTakeProfitOrderIfNecessary(loggedUser.Id, order.AssetId, order, now);
            using (var transaction = TransactionalDapperCommand)
            {
                transaction.Update(order);
                if (oldTakeProfitOrder != null)
                    transaction.Delete(oldTakeProfitOrder);
                if (newTakeProfitOrder != null)
                {
                    newTakeProfitOrder.OrderId = order.Id;
                    transaction.Insert(newTakeProfitOrder);
                }
                transaction.Commit();
            }
            return GetOrderResponse(loggedUser, order, AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id), new List<DomainObjects.Asset.Asset>() { asset });
        }

        public List<OrderResponse> CloseAll(int assetId)
        {
            var loggedUser = GetValidUser();
            var orders = Data.ListOrdersWithRelated(new int[] { loggedUser.Id }, new int[] { assetId }, new OrderStatusType[] { OrderStatusType.Executed }, null);
            if (!orders.Any())
                return new List<OrderResponse>();

            var currentValue = AssetCurrentValueBusiness.GetRealCurrentValue(assetId);
            if (currentValue == null)
                throw new InvalidOperationException("Asset doesn't have current value");

            var usdPosition = AdvisorProfitBusiness.ListAdvisorProfit(loggedUser.Id, new List<int>() { AssetUSDId }).First();
            var closeDate = Data.GetDateTimeNow();
            var tupleForOrderAndTakeProfitOrder = new List<Tuple<Order, Order>>();
            var valueClosed = 0.0;
            var closedOrders = new List<Order>();
            foreach (var order in orders)
            {
                Order closeOrder = null, takeProfitOrder = null;
                double closedDollar;
                var consideredPrice = order.OrderType == OrderType.Buy ? currentValue.BidValue : currentValue.AskValue;
                InternalCloseOrder(out closeOrder, out takeProfitOrder, out closedDollar, order, null, order.RelatedOrders, null, loggedUser.Id, consideredPrice, closeDate);
                valueClosed += closedDollar;
                tupleForOrderAndTakeProfitOrder.Add(new Tuple<Order, Order>(order, null));
                tupleForOrderAndTakeProfitOrder.Add(new Tuple<Order, Order>(closeOrder, takeProfitOrder));
                closedOrders.Add(closeOrder);
            }
            SetUsdPosition(usdPosition, OrderStatusType.Close, valueClosed, closeDate);

            var orderData = new Dictionary<int, List<Tuple<Order, Order>>>();
            orderData[assetId] = tupleForOrderAndTakeProfitOrder;
            SaveOrdersAndPositions(loggedUser.Id, closeDate, orderData, usdPosition, new Dictionary<int, double> { { assetId, currentValue.BidValue } }, 
                new Dictionary<int, double> { { assetId, currentValue.AskValue } }, null);

            var advisor = AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id);
            var assets = new List<DomainObjects.Asset.Asset>() { AssetBusiness.GetById(assetId) };
            var result = new List<OrderResponse>();
            foreach (var order in closedOrders)
                result.Add(GetOrderResponse(loggedUser, order, advisor, assets));
            
            return result;
        }

        internal bool GetUserHasAnyOrder(int userId)
        {
            return Data.GetAnyOrderCreatedByUser(userId, AssetUSDId) != null;
        }

        public OrderResponse CloseOrder(int orderId, double? quantity)
        {
            var loggedUser = GetValidUser();
            var order = Data.GetWithRelated(orderId);
            BaseValidationOnPositionUpdate(order, loggedUser);

            var currentValue = AssetCurrentValueBusiness.GetRealCurrentValue(order.AssetId);
            if (currentValue == null)
                throw new InvalidOperationException("Asset doesn't have current value");

            return InternalCloseOrder(order, null, order.RelatedOrders, quantity, loggedUser, currentValue.BidValue, currentValue.AskValue, Data.GetDateTimeNow());
        }

        private void BaseValidationOnPositionUpdate(Order order, User loggedUser)
        {
            BaseValidationOnOrderUpdate(order, loggedUser);
            if (order.OrderStatusType != OrderStatusType.Executed)
                throw new BusinessException("Order cannot be updated.");
        }

        private void BaseValidationOnOrderUpdate(Order order, User loggedUser)
        {
            if (order == null)
                throw new NotFoundException("Order not found.");
            if (order.UserId != loggedUser.Id)
                throw new UnauthorizedException("User unauthorized");
        }

        private void BaseValidationOnOpenOrderUpdate(Order order, User loggedUser)
        {
            BaseValidationOnOrderUpdate(order, loggedUser);
            if (order.OrderStatusType != OrderStatusType.Open || order.OrderId.HasValue)
                throw new BusinessException("Order cannot be updated.");
        }

        public OrderResponse CancelOrder(int orderId)
        {
            var loggedUser = GetValidUser();
            var order = Data.GetWithRelated(orderId);
            BaseValidationOnOpenOrderUpdate(order, loggedUser);

            var dateTime = Data.GetDateTimeNow();
            order.Status = OrderStatusType.Canceled.Value;
            order.StatusDate = dateTime;
            var canceledOrders = new List<Tuple<Order, Order>>();
            canceledOrders.Add(new Tuple<Order, Order>(order, null));
            foreach (var related in order.RelatedOrders)
            {
                related.Status = OrderStatusType.Canceled.Value;
                related.StatusDate = dateTime;
                canceledOrders.Add(new Tuple<Order, Order>(related, null));
            }

            var openOrders = canceledOrders.Where(c => !c.Item1.OrderId.HasValue);
            var releasedDollar = openOrders.Any() ? openOrders.Sum(c => c.Item1.Price * c.Item1.Quantity) : 0.0;
            AdvisorProfit usdPosition = null;
            if (releasedDollar > 0)
            {
                usdPosition = AdvisorProfitBusiness.ListAdvisorProfit(loggedUser.Id, new List<int>() { AssetUSDId }).First();
                SetUsdPosition(usdPosition, OrderStatusType.Canceled, releasedDollar, dateTime);
            }
            var currentValue = AssetCurrentValueBusiness.ListAllAssets(true, new int[] { order.AssetId }).First();
            var orderData = new Dictionary<int, List<Tuple<Order, Order>>>();
            orderData[order.AssetId] = canceledOrders;
            SaveOrdersAndPositions(loggedUser.Id, dateTime, orderData, usdPosition, new Dictionary<int, double> { { order.AssetId, currentValue.BidValue } }, 
                new Dictionary<int, double> { { order.AssetId, currentValue.AskValue } }, null);

            return GetOrderResponse(loggedUser, order, AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id), new List<DomainObjects.Asset.Asset>() { AssetBusiness.GetById(order.AssetId) });
        }

        public List<OrderResponse> CancelAllOpen(int? assetId)
        {
            var loggedUser = GetValidUser();
            var assetFilter = assetId.HasValue ? new int[] { assetId.Value } : null; 
            var orders = ListOrders(new int[] { loggedUser.Id }, assetFilter, new OrderStatusType[] { OrderStatusType.Open }, null);
            if (!orders.Any(c => !c.OrderId.HasValue))
                return new List<OrderResponse>();

            var dateTime = Data.GetDateTimeNow();
            var canceledOrders = new List<Tuple<Order, Order>>();
            foreach (var order in orders)
            {
                if (!order.OrderId.HasValue || orders.Any(c => c.Id == order.OrderId.Value))
                {
                    order.Status = OrderStatusType.Canceled.Value;
                    order.StatusDate = dateTime;
                    canceledOrders.Add(new Tuple<Order, Order>(order, null));
                }
            }

            var openOrders = orders.Where(c => !c.OrderId.HasValue);
            var releasedDollar = openOrders.Any() ? openOrders.Sum(c => c.Price * c.Quantity) : 0.0;
            AdvisorProfit usdPosition = null;
            if (releasedDollar > 0)
            {
                usdPosition = AdvisorProfitBusiness.ListAdvisorProfit(loggedUser.Id, new List<int>() { AssetUSDId }).First();
                SetUsdPosition(usdPosition, OrderStatusType.Canceled, releasedDollar, dateTime);
            }
            var orderData = new Dictionary<int, List<Tuple<Order, Order>>>();
            foreach (var cancelled in canceledOrders)
            {
                if (!orderData.ContainsKey(cancelled.Item1.AssetId))
                    orderData[cancelled.Item1.AssetId] = new List<Tuple<Order, Order>>();
                orderData[cancelled.Item1.AssetId].Add(cancelled);
            }

            var assetsCurrentValues = AssetCurrentValueBusiness.ListAllAssets(true, orderData.Keys.Select(c => c));
            SaveOrdersAndPositions(loggedUser.Id, dateTime, orderData, usdPosition, assetsCurrentValues.ToDictionary(c => c.Id, c => c.BidValue), 
                assetsCurrentValues.ToDictionary(c => c.Id, c => c.AskValue), null);

            var advisor = AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id);
            var assets = AssetBusiness.ListAssets(false, orders.Select(c => c.AssetId).Distinct());
            var result = new List<OrderResponse>();
            foreach (var tuple in canceledOrders)
            {
                if (!tuple.Item1.OrderId.HasValue)
                    result.Add(GetOrderResponse(loggedUser, tuple.Item1, advisor, assets));
            }
            return result;
        }

        private OrderResponse InternalCloseOrder(Order order, OrderActionType actionType, List<Order> relatedOrders, double? quantity, User loggedUser, double assetBidValue,
            double assetAskValue, DateTime dateTime)
        {
            Order closedOrder, takeProfitOrder;
            double closedDollar;
            double consideredPrice;
            if (actionType == OrderActionType.StopLoss)
                consideredPrice = order.StopLoss.Value;
            else
                consideredPrice = order.OrderType == OrderType.Buy ? assetBidValue : assetAskValue;

            InternalCloseOrder(out closedOrder, out takeProfitOrder, out closedDollar, order, actionType, relatedOrders, quantity, loggedUser.Id, consideredPrice, dateTime);

            var usdPosition = AdvisorProfitBusiness.ListAdvisorProfit(loggedUser.Id, new List<int>() { AssetUSDId }).First();
            SetUsdPosition(usdPosition, OrderStatusType.Close, closedDollar, dateTime);

            var orderData = new Dictionary<int, List<Tuple<Order, Order>>>();
            orderData[order.AssetId] = new List<Tuple<Order, Order>>() { new Tuple<Order, Order>(order, null), new Tuple<Order, Order>(closedOrder, takeProfitOrder) };
            SaveOrdersAndPositions(loggedUser.Id, dateTime.Date, orderData, usdPosition, new Dictionary<int, double> { { order.AssetId, assetBidValue } }, 
                new Dictionary<int, double> { { order.AssetId, assetAskValue } }, null);

            return GetOrderResponse(loggedUser, closedOrder, AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id), new List<DomainObjects.Asset.Asset>() { AssetBusiness.GetById(order.AssetId) });
        }

        private void InternalCloseOrder(out Order closeOrder, out Order takeProfitOrder, out double closedDollar, Order order, OrderActionType actionType, List<Order> relatedOrders, double? quantity, 
            int loggedUserId, double assetCurrentValue, DateTime dateTime)
        {
            if (!quantity.HasValue)
                quantity = order.RemainingQuantity;

            if (quantity <= 0)
                throw new BusinessException("Invalid amount.");

            if (order.RemainingQuantity < quantity)
                throw new BusinessException("Insufficient funds.");

            double profitWithoutFee, profit, fee, totalTradeFee;
            GetCloseData(out profitWithoutFee, out profit, out fee, out totalTradeFee, out closedDollar, order.OrderType, order.Price, assetCurrentValue, quantity.Value, order.Quantity, order.Fee.Value);

            closeOrder = CreateNewOrder(loggedUserId, order.AssetId, order.OrderType.GetOppositeType(), OrderStatusType.Close, actionType ?? order.OrderActionType, quantity.Value, assetCurrentValue, dateTime, null, null, profit, order.Id, order.StatusDate, 0, profitWithoutFee, fee, totalTradeFee);
            takeProfitOrder = relatedOrders.SingleOrDefault(o => o.OrderStatusType == OrderStatusType.Open);
            if (takeProfitOrder != null)
            {
                if (quantity == order.RemainingQuantity)
                {
                    takeProfitOrder.Status = OrderStatusType.Canceled.Value;
                    takeProfitOrder.StatusDate = dateTime;
                }
                else
                {
                    takeProfitOrder.Quantity -= quantity.Value;
                    takeProfitOrder.RemainingQuantity -= quantity.Value;
                }
            }
            if (quantity == order.RemainingQuantity)
                order.Status = OrderStatusType.Finished.Value;

            order.RemainingQuantity -= quantity.Value;
        }

        private Order CreateNewOrder(int loggedUserId, int assetId, OrderType type, OrderStatusType orderStatus, OrderActionType actionType, double quantity, 
            double price, DateTime creationDate, double? takeProfit, double? stopLoss, double? profit, int? orderId, DateTime? openDate, double remainingQuantity,
            double? profitWithoutFee, double? fee, double? totalTradeFee)
        {
            return new Order()
            {
                AssetId = assetId,
                CreationDate = creationDate,
                UserId = loggedUserId,
                Quantity = quantity,
                RemainingQuantity = remainingQuantity,
                Price = price,
                Status = orderStatus.Value,
                StatusDate = creationDate,
                StopLoss = stopLoss,
                TakeProfit = takeProfit,
                Type = type.Value,
                Profit = profit,
                OrderId = orderId,
                ActionType = actionType.Value,
                OpenDate = openDate,
                Fee = fee,
                TotalTradeFee = totalTradeFee,
                ProfitWithoutFee = profitWithoutFee
            };
        }

        private void BaseValidationForOrderValues(double quantity, double? price, double? takeProfit, double? stopLoss)
        {
            if (quantity <= 0)
                throw new BusinessException("Invalid quantity.");
            if (price <= 0)
                throw new BusinessException("Invalid price.");
            if (takeProfit <= 0)
                throw new BusinessException("Invalid take profit value.");
            if (stopLoss <= 0)
                throw new BusinessException("Invalid stop loss value.");
        }

        public OrderResponse EditOrder(int orderId, double quantity, double? price, double? takeProfit, double? stopLoss)
        {
            var loggedUser = GetValidUser();
            var order = Data.GetWithRelated(orderId);
            BaseValidationOnOpenOrderUpdate(order, loggedUser);
            BaseValidationForOrderValues(quantity, price, takeProfit, stopLoss);

            var asset = AssetBusiness.GetById(order.AssetId);
            if (!asset.Enabled)
                throw new BusinessException("Invalid asset.");

            var usdPosition = AdvisorProfitBusiness.ListAdvisorProfit(loggedUser.Id, new List<int>() { AssetUSDId }).First();
            var currentValue = AssetCurrentValueBusiness.GetRealCurrentValue(order.AssetId);
            if (currentValue == null)
                throw new InvalidOperationException($"Asset {order.AssetId} value cannot be found.");

            var consideredPrice = order.OrderType == OrderType.Buy ? currentValue.AskValue : currentValue.BidValue;
            if (!price.HasValue || IsOrderPriceLowerThanMarket(order.OrderType, price.Value, consideredPrice))
                price = consideredPrice;

            if (order.OrderType == OrderType.Sell)
            {
                var forcedStopLoss = price.Value * 2;
                if (!stopLoss.HasValue)
                    stopLoss = forcedStopLoss;
                else if (stopLoss.Value > forcedStopLoss)
                    throw new BusinessException($"The maximum stop loss value is {Util.Util.GetFormattedValue(forcedStopLoss)}.");
            }

            if (stopLoss.HasValue)
            {
                if (price == consideredPrice && ((order.OrderType == OrderType.Buy && currentValue.BidValue <= stopLoss.Value) || (order.OrderType == OrderType.Sell && currentValue.AskValue >= stopLoss.Value)))
                    throw new BusinessException($"Invalid stop loss value for current price {Util.Util.GetFormattedValue(order.OrderType == OrderType.Buy ? currentValue.BidValue : currentValue.AskValue)}.");
                else if ((order.OrderType == OrderType.Buy && price <= stopLoss.Value) || (order.OrderType == OrderType.Sell && price >= stopLoss.Value))
                    throw new BusinessException($"Invalid stop loss value for the price {Util.Util.GetFormattedValue(price.Value)}.");
            }
            if (takeProfit.HasValue)
            {
                if (price == consideredPrice && ((order.OrderType == OrderType.Buy && currentValue.BidValue >= takeProfit.Value) || (order.OrderType == OrderType.Sell && currentValue.AskValue <= takeProfit.Value)))
                    throw new BusinessException($"Invalid take profit value for current price {Util.Util.GetFormattedValue(order.OrderType == OrderType.Buy ? currentValue.BidValue : currentValue.AskValue)}.");
                else if ((order.OrderType == OrderType.Buy && price >= takeProfit.Value) || (order.OrderType == OrderType.Sell && price <= takeProfit.Value))
                    throw new BusinessException($"Invalid take profit value for the price {Util.Util.GetFormattedValue(price.Value)}.");
            }
            
            var now = Data.GetDateTimeNow();
            var oldTakeProfitOrder = order.RelatedOrders.FirstOrDefault(c => c.OrderStatusType == OrderStatusType.Open);
            var releasedAmount = !order.OrderId.HasValue ? order.Price * order.Quantity : 0.0;
            if (releasedAmount > 0)
                SetUsdPosition(usdPosition, OrderStatusType.Canceled, releasedAmount, now);

            if (usdPosition.TotalDollar < (quantity * price))
                throw new BusinessException("Insufficient funds.");

            if (price == consideredPrice)
            {
                order.Status = OrderStatusType.Executed.Value;
                order.ActionType = OrderActionType.Market.Value;
                order.StatusDate = now;
                order.Fee = quantity * price * OrderFee;
                order.Quantity = quantity * (1 - OrderFee);
            }
            else
                order.Quantity = quantity;

            order.RemainingQuantity = order.Quantity;
            order.Price = price.Value;
            order.StopLoss = stopLoss;
            order.TakeProfit = takeProfit;
            var newTakeProfitOrder = CreateTakeProfitOrderIfNecessary(loggedUser.Id, order.AssetId, order, now);
            SetUsdPosition(usdPosition, order.OrderStatusType, quantity * price.Value, now);

            var orderData = new Dictionary<int, List<Tuple<Order, Order>>>();
            orderData[order.AssetId] = new List<Tuple<Order, Order>>() { new Tuple<Order, Order>(order, newTakeProfitOrder) };
            SaveOrdersAndPositions(loggedUser.Id, now, orderData, usdPosition, new Dictionary<int, double> { { order.AssetId, currentValue.BidValue } }, 
                new Dictionary<int, double> { { order.AssetId, currentValue.AskValue } }, oldTakeProfitOrder);

            return GetOrderResponse(loggedUser, order, AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id), new List<DomainObjects.Asset.Asset>() { asset });
        }

        public OrderResponse CreateOrder(int assetId, OrderType type, double quantity, double? price, double? takeProfit, double? stopLoss)
        {
            var loggedUser = GetValidUser();
            BaseValidationForOrderValues(quantity, price, takeProfit, stopLoss);

            var asset = AssetBusiness.GetById(assetId);
            if (!asset.Enabled)
                throw new BusinessException("Invalid asset.");
            if (!asset.ShortSellingEnabled && type == OrderType.Sell)
                throw new BusinessException("Invalid short operation for this market.");

            var usdPosition = AdvisorProfitBusiness.ListAdvisorProfit(loggedUser.Id, new List<int>() { AssetUSDId }).First();

            var currentValue = AssetCurrentValueBusiness.GetRealCurrentValue(assetId);
            if (currentValue == null)
                throw new InvalidOperationException($"Asset {assetId} value cannot be found.");

            var consideredPrice = type == OrderType.Buy ? currentValue.AskValue : currentValue.BidValue;
            if (!price.HasValue || IsOrderPriceLowerThanMarket(type, price.Value, consideredPrice))
                price = consideredPrice;

            if (usdPosition.TotalDollar < (quantity * price))
                throw new BusinessException("Insufficient funds.");

            if (type == OrderType.Sell)
            {
                var forcedStopLoss = price.Value * 2;
                if (!stopLoss.HasValue)
                    stopLoss = forcedStopLoss;
                else if (stopLoss.Value > forcedStopLoss)
                    throw new BusinessException($"The maximum stop loss value is {Util.Util.GetFormattedValue(forcedStopLoss)}.");
            }

            if (stopLoss.HasValue)
            {
                if (price == consideredPrice && ((type == OrderType.Buy && currentValue.BidValue <= stopLoss.Value) || (type == OrderType.Sell && currentValue.AskValue >= stopLoss.Value)))
                    throw new BusinessException($"Invalid stop loss value for current price {Util.Util.GetFormattedValue(type == OrderType.Buy ? currentValue.BidValue : currentValue.AskValue)}.");
                else if ((type == OrderType.Buy && price <= stopLoss.Value) || (type == OrderType.Sell && price >= stopLoss.Value))
                    throw new BusinessException($"Invalid stop loss value for the price {Util.Util.GetFormattedValue(price.Value)}.");
            }
            if (takeProfit.HasValue)
            {
                if (price == consideredPrice && ((type == OrderType.Buy && currentValue.BidValue >= takeProfit.Value) || (type == OrderType.Sell && currentValue.AskValue <= takeProfit.Value)))
                    throw new BusinessException($"Invalid take profit value for current price {Util.Util.GetFormattedValue(type == OrderType.Buy ? currentValue.BidValue : currentValue.AskValue)}.");
                else if ((type == OrderType.Buy && price >= takeProfit.Value) || (type == OrderType.Sell && price <= takeProfit.Value))
                    throw new BusinessException($"Invalid take profit value for the price {Util.Util.GetFormattedValue(price.Value)}.");
            }

            OrderStatusType orderStatusType;
            OrderActionType orderActionType;
            double? fee = null;
            var orderQuantity = quantity;
            if (price == consideredPrice)
            {
                orderStatusType = OrderStatusType.Executed;
                orderActionType = OrderActionType.Market;
                fee = quantity * price * OrderFee;
                orderQuantity = quantity * (1 - OrderFee);
            }
            else
            {
                orderStatusType = OrderStatusType.Open;
                orderActionType = OrderActionType.Limit;
            }
            var creationDate = Data.GetDateTimeNow();
            SetUsdPosition(usdPosition, orderStatusType, quantity * price.Value, creationDate);
            var order = CreateNewOrder(loggedUser.Id, assetId, type, orderStatusType, orderActionType, orderQuantity, price.Value, creationDate, takeProfit, stopLoss, null, null, null, orderQuantity, null, fee, null);
            var takeProfitOrder = CreateTakeProfitOrderIfNecessary(loggedUser.Id, assetId, order, creationDate);

            var orderData = new Dictionary<int, List<Tuple<Order, Order>>>();
            orderData[order.AssetId] = new List<Tuple<Order, Order>>() { new Tuple<Order, Order>(order, takeProfitOrder) };
            SaveOrdersAndPositions(loggedUser.Id, creationDate, orderData, usdPosition, new Dictionary<int, double> { { order.AssetId, currentValue.BidValue } }, 
                new Dictionary<int, double> { { order.AssetId, currentValue.AskValue } }, null);

            return GetOrderResponse(loggedUser, order, AdvisorRankingBusiness.GetAdvisorSimpleData(loggedUser.Id), new List<DomainObjects.Asset.Asset>() { asset });
        }

        private OrderResponse ExecuteOrderIfNecessary(Order order, TickerDataModel assetCurrentPrice, DateTime currentDate)
        {
            var user = new User() { Id = order.UserId };
            if (order.OrderStatusType == OrderStatusType.Executed && order.StopLoss.HasValue)
            {
                if ((order.OrderType == OrderType.Buy && assetCurrentPrice.BidValue <= order.StopLoss.Value) ||
                    (order.OrderType == OrderType.Sell && assetCurrentPrice.AskValue >= order.StopLoss.Value))
                {
                    return InternalCloseOrder(order, OrderActionType.StopLoss, order.RelatedOrders, null, user, assetCurrentPrice.BidValue, assetCurrentPrice.AskValue, currentDate);
                }
            }
            else if (order.OrderStatusType == OrderStatusType.Open)
            {
                double price = order.OrderType == OrderType.Buy ? assetCurrentPrice.AskValue : assetCurrentPrice.BidValue;
                if ((order.OrderType == OrderType.Buy && price <= order.Price) || (order.OrderType == OrderType.Sell && price >= order.Price))
                {
                    AdvisorProfit usdPosition = null;
                    var orderData = new Dictionary<int, List<Tuple<Order, Order>>>();
                    if (!order.OrderId.HasValue)
                    {
                        order.Status = OrderStatusType.Executed.Value;
                        order.StatusDate = currentDate;
                        order.Fee = order.Quantity * order.Price * OrderFee;
                        order.Quantity = order.Quantity * (1 - OrderFee);
                        order.RemainingQuantity = order.Quantity;

                        var takeProfitOrder = order.RelatedOrders.FirstOrDefault();
                        if (takeProfitOrder != null)
                        {
                            takeProfitOrder.Quantity = order.Quantity;
                            takeProfitOrder.RemainingQuantity = order.Quantity;
                        }
                        orderData[order.AssetId] = new List<Tuple<Order, Order>>() { new Tuple<Order, Order>(order, takeProfitOrder) };
                    }
                    else
                    {
                        var parentOrder = order.RelatedOrders.Single();
                        parentOrder.Status = OrderStatusType.Finished.Value;
                        parentOrder.RemainingQuantity -= order.Quantity;
                        order.Status = OrderStatusType.Close.Value;
                        order.ActionType = OrderActionType.TakeProfit.Value;
                        order.OpenDate = parentOrder.StatusDate;
                        order.StatusDate = currentDate;
                        order.RemainingQuantity = 0;

                        double profitWithoutFee, profit, fee, totalTradeFee, closedDollar;
                        GetCloseData(out profitWithoutFee, out profit, out fee, out totalTradeFee, out closedDollar, parentOrder.OrderType, parentOrder.Price, order.Price, order.Quantity, parentOrder.Quantity, parentOrder.Fee.Value);
                        order.ProfitWithoutFee = profitWithoutFee;
                        order.Fee = fee;
                        order.Profit = profit;
                        order.TotalTradeFee = totalTradeFee;

                        usdPosition = AdvisorProfitBusiness.ListAdvisorProfit(order.UserId, new List<int>() { AssetUSDId }).First();
                        SetUsdPosition(usdPosition, OrderStatusType.Close, closedDollar, currentDate);
                        orderData[order.AssetId] = new List<Tuple<Order, Order>>() { new Tuple<Order, Order>(parentOrder, order) };
                    }
                    SaveOrdersAndPositions(order.UserId, currentDate, orderData, usdPosition, new Dictionary<int, double> { { order.AssetId, assetCurrentPrice.BidValue } }, 
                        new Dictionary<int, double> { { order.AssetId, assetCurrentPrice.AskValue } }, null);
                    return GetOrderResponse(user, order, AdvisorRankingBusiness.GetAdvisorSimpleData(user.Id), new List<DomainObjects.Asset.Asset>() { AssetBusiness.GetById(order.AssetId) });
                }
            }
            return null;
        }

        private void GetCloseData(out double profitWithoutFee, out double profit, out double fee, out double totalTradeFee, out double closedDollar, 
            OrderType parentOrderType, double parentPrice, double closePrice, double quantity, double parentQuantity, double parentFee)
        {
            var expectedValue = GetExpectedCloseValue(parentOrderType, parentPrice, closePrice, quantity);
            profitWithoutFee = GetProfitValue(parentOrderType, parentPrice, closePrice);
            fee = expectedValue * OrderFee;
            closedDollar = expectedValue - fee;
            totalTradeFee = fee + (parentFee * quantity / parentQuantity);
            profit = GetProfitValue(closedDollar, parentPrice, quantity, parentFee / (parentQuantity * parentPrice));
        }

        public double GetProfitValue(double closedDollar, double parentPrice, double quantity, double parentFeePercentage)
        {
            return closedDollar / (parentPrice * quantity / (1 - parentFeePercentage)) - 1;
        }

        public double GetExpectedCloseValue(OrderType parentOrderType, double? parentPrice, double closePrice, double quantity)
        {
            return parentOrderType == OrderType.Buy ? closePrice * quantity : quantity * (2 * parentPrice.Value - closePrice);
        }

        public double GetProfitValue(OrderType orderType, double startPrice, double closePrice)
        {
            return (orderType == OrderType.Buy ? 1.0 : -1.0) * (closePrice / startPrice - 1);
        }

        private void SetUsdPosition(AdvisorProfit usdPosition, OrderStatusType orderStatusType, double value, DateTime dateTime)
        {
            if (orderStatusType == OrderStatusType.Close || orderStatusType == OrderStatusType.Canceled)
                usdPosition.TotalDollar += value;
            else
                usdPosition.TotalDollar -= value;

            usdPosition.TotalQuantity = usdPosition.TotalDollar;
            usdPosition.UpdateDate = dateTime;
        }

        private Order CreateTakeProfitOrderIfNecessary(int loggedUserId, int assetId, Order order, DateTime creationDate)
        {
            if (order.TakeProfit.HasValue)
            {
                var profitOrderType = order.OrderType.GetOppositeType();
                return CreateNewOrder(loggedUserId, assetId, profitOrderType, OrderStatusType.Open, OrderActionType.Automated, order.Quantity, order.TakeProfit.Value, creationDate, null, null, null, null, null, order.Quantity, null, null, null);
            }
            return null;
        }

        internal Dictionary<int, Dictionary<OrderActionType, List<OrderResponse>>> ClosePositionForStopLossAndTargetPriceReached(DateTime currentDate, Dictionary<int, TickerDataModel> values)
        {
            var result = new Dictionary<int, Dictionary<OrderActionType, List<OrderResponse>>>();
            var orders = Data.ListOpenOrdersAndExecutedWithStopLoss(values.Keys);
            foreach (var order in orders)
            {
                OrderResponse response = null;
                try
                {
                    response = ExecuteOrderIfNecessary(order, values[order.AssetId], currentDate);
                }
                catch(Exception ex)
                {
                    EmailBusiness.SendErrorEmailAsync($"Error on ClosePositionForStopLossAndTargetPriceReached - OrderId {order.Id}, User {order.UserId}", ex: ex);
                }
                if (response != null)
                {
                    var actionType = OrderActionType.Get(response.ActionType);
                    if (!result.ContainsKey(order.UserId))
                        result[order.UserId] = new Dictionary<OrderActionType, List<OrderResponse>>();
                    if (!result[order.UserId].ContainsKey(actionType))
                        result[order.UserId][actionType] = new List<OrderResponse>();

                    result[order.UserId][actionType].Add(response);
                }
            }
            return result;
        }

        private void SaveOrdersAndPositions(int userId, DateTime baseDatetime, Dictionary<int, List<Tuple<Order, Order>>> assetTupleForOrderAndTakeProfitOrderForUser, 
            AdvisorProfit usdAdvisorProfit, Dictionary<int, double> assetsBidValues, Dictionary<int, double> assetsAskValues, Order orderToDelete)
        {
            var newAdvisorAssetProfit = new Dictionary<int, List<AdvisorProfit>>();
            using (var transaction = TransactionalDapperCommand)
            {
                if (orderToDelete != null)
                    transaction.Delete(orderToDelete);

                foreach (var assetData in assetTupleForOrderAndTakeProfitOrderForUser)
                {
                    foreach (var assetOrder in assetData.Value)
                    {
                        if (assetOrder.Item1.Id > 0)
                            transaction.Update(assetOrder.Item1);
                        else
                            transaction.Insert(assetOrder.Item1);

                        if (assetOrder.Item2 != null)
                        {
                            if (assetOrder.Item2.Id > 0)
                                transaction.Update(assetOrder.Item2);
                            else
                            {
                                assetOrder.Item2.OrderId = assetOrder.Item1.Id;
                                transaction.Insert(assetOrder.Item2);
                            }
                        }
                    }
                    SetTransactionOnData(transaction);
                    var orders = Data.ListOrdersForRankingProfitCalculation(new int[] { userId }, new int[] { assetData.Key });
                    newAdvisorAssetProfit[assetData.Key] = AdvisorRankingBusiness.GetAdvisorRankingAndProfitData(userId, assetData.Key, baseDatetime, orders.Where(c => c.AssetId == assetData.Key).ToList(), assetsBidValues[assetData.Key], assetsAskValues[assetData.Key]);

                    transaction.Execute($"DELETE FROM [AdvisorProfit] WHERE UserId = {userId} AND AssetId = {assetData.Key}", null);
                    foreach (var advisorProfit in newAdvisorAssetProfit.Values.First())
                        transaction.Insert(advisorProfit);
                }

                if (usdAdvisorProfit != null)
                    transaction.Update(usdAdvisorProfit);

                transaction.Commit();
            }

            var advisorsCache = AdvisorRankingBusiness.ListAdvisorsFullData();
            var advisor = advisorsCache.FirstOrDefault(c => c.Id == userId);
            if (advisor != null)
            {
                var assetsToUpdate = newAdvisorAssetProfit.SelectMany(c => c.Value).ToList();
                if (usdAdvisorProfit != null)
                    assetsToUpdate.Add(usdAdvisorProfit);

                if (assetsToUpdate.Any())
                {
                    var assetsId = assetsToUpdate.Select(c => c.AssetId).Distinct().ToHashSet();
                    advisor.AdvisorProfit.RemoveAll(c => assetsId.Contains(c.AssetId));
                    advisor.AdvisorProfit.AddRange(assetsToUpdate);
                }
            }
        }

        private bool IsOrderPriceLowerThanMarket(OrderType type, double price, double currentValue)
        {   
            if (type == OrderType.Buy)
                return price >= currentValue;
            return price <= currentValue;
        }

        public List<Order> ListOrders(IEnumerable<int> usersId = null, IEnumerable<int> assetsId = null, IEnumerable<OrderStatusType> ordersStatusType = null, 
            OrderType orderType = null)
        {
            return Data.ListOrders(usersId, assetsId, ordersStatusType, orderType);
        }

        public List<Order> ListOrdersForRankingProfitCalculation(IEnumerable<int> usersId)
        {
            return Data.ListOrdersForRankingProfitCalculation(usersId, null);
        }

        public List<OrderResponse> ListLastAdvisorsOrdersForAsset(int assetId)
        {
            List<Order> orders = null;
            List<DomainObjects.Asset.Asset> assets = null;
            List<AdvisorRanking> advisors = null;
            Parallel.Invoke(() => orders = Data.ListLastAdvisorsOrdersForAsset(assetId, new OrderStatusType[] { OrderStatusType.Executed, OrderStatusType.Close }),
                            () => assets = assets = AssetBusiness.ListAssets(false, new int[] { assetId }),
                            () => advisors = AdvisorRankingBusiness.ListAdvisorsFullData());

            var loggedUser = GetLoggedUser();
            var result = new List<OrderResponse>();
            foreach (var order in orders)
            {
                var advisor = advisors.FirstOrDefault(c => c.Id == order.UserId);
                result.Add(GetOrderResponse(loggedUser, order, advisor, assets));
            }
            return result;
        }

        public List<OrderResponse> ListAdvisorOrders(int advisorId, int? assetId, int[] status, int? type)
        {
            var assetFilter = assetId.HasValue ? new int[] { assetId.Value } : null;
            var statusType = status?.Select(s => OrderStatusType.Get(s));
            List<Order> orders = null;
            AdvisorRanking advisor = null;
            Parallel.Invoke(() => orders = ListOrders(new int[] { advisorId }, assetFilter, statusType, type.HasValue ? OrderType.Get(type.Value) : null).
                Where(o => !o.OrderId.HasValue || o.OrderStatusType == OrderStatusType.Close).OrderByDescending(c => c.StatusDate).ToList(),
                            () => advisor = AdvisorRankingBusiness.GetAdvisorFullData(advisorId));

            List <DomainObjects.Asset.Asset> assets = null;
            if (orders.Any())
            {
                assets = AssetBusiness.ListAssets(false, assetFilter);
                var openPositions = orders.Where(c => c.OrderStatusType == OrderStatusType.Executed);
                if (openPositions.Any())
                {
                    var assetsId = openPositions.Select(c => c.AssetId).Distinct().ToHashSet();
                    var assetsValue = AssetCurrentValueBusiness.ListAllAssets(true, assetsId);
                    orders.ForEach(c =>
                    {
                        if (openPositions.Any(o => o.Id == c.Id))
                        {
                            var asset = assetsValue.FirstOrDefault(a => a.Id == c.AssetId);
                            if (asset != null)
                            {
                                c.Profit = (asset.CurrentValue - c.Price) / c.Price;
                                if (c.OrderType.Equals(OrderType.Sell))
                                    c.Profit *= -1;
                            }
                        }
                    });
                }
            }

            var loggedUser = GetLoggedUser(); 
            var result = new List<OrderResponse>();
            foreach(var order in orders)
                result.Add(GetOrderResponse(loggedUser, order, advisor, assets));
            
            return result;
        }

        public List<OrderResponse> ListFollowedTrades()
        {
            var loggedUser = GetValidUser();

            List<Order> orders = null;
            List<DomainObjects.Asset.Asset> assets = null;
            List<AdvisorRanking> advisors = null;
            Parallel.Invoke(() => orders = Data.ListUsersOrdersByDate(Data.GetDateTimeNow().AddDays(-14), new OrderStatusType[] { OrderStatusType.Executed, OrderStatusType.Close }, loggedUser.FollowedAdvisors, 5, AssetUSDId),
                            () => assets = assets = AssetBusiness.ListAssets(false),
                            () => advisors = AdvisorRankingBusiness.ListAdvisorsFullData());

            return orders.Select(c => GetOrderResponse(loggedUser, c, advisors.FirstOrDefault(a => a.Id == c.UserId), assets)).ToList();
        }

        public double? GetOpenPrice(Order order)
        {
            if (OrderStatusType.Executed == order.OrderStatusType)
                return order.Price;
            else if (OrderStatusType.Close == order.OrderStatusType)
            {
                if (OrderType.Buy == order.OrderType.GetOppositeType())
                    return order.Price / (1 + order.ProfitWithoutFee);
                else
                    return order.Price / (1 - order.ProfitWithoutFee);
            }
            else
                return null;
        }

        private OrderResponse GetOrderResponse(User loggedUser, Order order, AdvisorRanking advisorRanking, List<DomainObjects.Asset.Asset> assets)
        {
            var asset = assets.FirstOrDefault(c => c.Id == order.AssetId);
            var openPrice = GetOpenPrice(order);
            return new OrderResponse()
            {
                AssetId = order.AssetId,
                AssetCode = asset?.Code,
                AssetName = asset?.Name,
                ShortSellingEnabled = asset?.ShortSellingEnabled,
                Pair =  asset != null ? PairBusiness.GetBaseQuotePair(order.AssetId) : null,
                CreationDate = order.CreationDate,
                Id = order.Id,
                OrderId = order.OrderId,
                Price = order.Price,
                Quantity = order.Quantity,
                RemainingQuantity = order.RemainingQuantity,
                OpenDate = order.OpenDate,
                OpenPrice = openPrice,
                Invested = order.Price * order.Quantity,
                Status = order.Status,
                StatusDate = order.StatusDate,
                StopLoss = order.StopLoss,
                TakeProfit = order.TakeProfit,
                Type = order.Type,
                ActionType = order.ActionType,
                Profit = order.Profit,
                ProfitValue = !order.Profit.HasValue ? (double?)null : order.Profit.Value * openPrice * (order.OrderStatusType == OrderStatusType.Executed ? order.RemainingQuantity : order.Quantity),
                Fee = order.Fee,
                ProfitWithoutFee = order.ProfitWithoutFee,
                ProfitWithoutFeeValue = !order.ProfitWithoutFee.HasValue ? (double?)null : order.ProfitWithoutFee.Value * openPrice * (order.OrderStatusType == OrderStatusType.Executed ? order.RemainingQuantity : order.Quantity),
                AdvisorId = order.UserId,
                AdvisorName = advisorRanking?.Name,
                AdvisorDescription = advisorRanking?.Description,
                AdvisorGuid = advisorRanking?.UrlGuid.ToString(),
                AdvisorRanking = advisorRanking?.Ranking,
                AdvisorRating = advisorRanking?.Rating,
                FollowingAsset = loggedUser?.FollowedAssets?.Any(c => c == order.AssetId) == true,
                FollowingAdvisor = loggedUser?.FollowedAdvisors?.Any(c => c == order.UserId) == true,
                CanBeEdited = loggedUser != null && loggedUser.Id == order.UserId && 
                    (order.OrderStatusType == OrderStatusType.Executed || (order.OrderStatusType == OrderStatusType.Open && !order.OrderId.HasValue))
            };
        }

        public IEnumerable<int> ListTrendingAssetIdsBasedOnOrders(int[] orderStatusList, int resultSizeLimit, int numberOfDays)
        {
            return Data.ListTrendingAssetIdsBasedOnOrders(orderStatusList, resultSizeLimit, numberOfDays);
        }
    }
}
