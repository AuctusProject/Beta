﻿using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Trade;
using Auctus.DomainObjects.Trade;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Trade
{
    public class OrderData : BaseSql<Order>, IOrderData<Order>
    {
        public override string TableName => "Order";

        private const string SQL_LIST = @"SELECT o.* FROM [Order] o WITH(NOLOCK) {0}";

        private const string SQL_LIST_FOR_CALCULATION = @"SELECT o.* FROM [Order] o WITH(NOLOCK)
                                                        WHERE (o.Status = @CloseStatus OR o.Status = @ExecutedStatus OR (o.Status = @OpenStatus AND o.OrderId IS NULL)) 
                                                        AND {0}";

        private const string SQL_LIST_EXECUTED_WITH_STOP_LOSS_AND_OPEN = @"SELECT o.*, ro.* FROM [Order] o WITH(NOLOCK) LEFT JOIN [Order] ro WITH(NOLOCK) ON ro.Id = o.OrderId
                                                                           WHERE o.Status = @OpenStatus AND {0}
                                                                           UNION
                                                                           SELECT o.*, ro.* FROM [Order] o WITH(NOLOCK) LEFT JOIN [Order] ro WITH(NOLOCK) ON ro.OrderId = o.Id
                                                                           WHERE o.Status = @ExecutedStatus AND o.StopLoss IS NOT NULL AND {0}";

        private const string SQL_LAST_ADVISORS_ORDERS_FOR_ASSET = @"SELECT o.* FROM 
                                                                    [Order] o WITH(NOLOCK) 
                                                                    INNER JOIN (
	                                                                    SELECT o2.UserId, max(o2.StatusDate) StatusDate FROM [Order] o2 WITH(NOLOCK) WHERE o2.AssetId = @AssetId GROUP BY o2.UserId
                                                                    ) b ON b.UserId = o.UserId AND b.StatusDate = o.StatusDate
                                                                    WHERE 
                                                                    o.AssetId = @AssetId {0}";

        private const string SQL_GET_WITH_RELATED = "SELECT o.*, ro.* FROM [Order] o WITH(NOLOCK) LEFT JOIN [Order] ro WITH(NOLOCK) ON ro.OrderId = o.Id WHERE o.Id = @Id";

        private const string SQL_ANY_ORDER_CREATED_BY_USER = "SELECT TOP 1 o.* FROM [Order] o WITH(NOLOCK) WHERE o.UserId = @UserId AND o.AssetId <> @AssetId";

        public OrderData(IConfigurationRoot configuration) : base(configuration) { }

        private void BuildListOrders(ref string sql, out DynamicParameters parameters, IEnumerable<int> usersId, IEnumerable<int> assetsId, IEnumerable<OrderStatusType> ordersStatusType, OrderType orderType)
        {
            parameters = new DynamicParameters();
            var complement = usersId?.Any() == true || assetsId?.Any() == true || ordersStatusType?.Any() == true || orderType != null ? " WHERE " : "";
            if (usersId?.Any() == true)
            {
                complement += $"({string.Join(" OR ", usersId.Select((c, i) => $"o.UserId = @UserId{i}"))})";
                for (int i = 0; i < usersId.Count(); ++i)
                    parameters.Add($"UserId{i}", usersId.ElementAt(i), DbType.Int32);
            }
            if (assetsId?.Any() == true)
            {
                if (usersId?.Any() == true)
                    complement += " AND ";
                         
                complement += $"({string.Join(" OR ", assetsId.Select((c, i) => $"o.AssetId = @AssetId{i}"))})";
                for (int i = 0; i < assetsId.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsId.ElementAt(i), DbType.Int32);
            }
            if (ordersStatusType?.Any() == true)
            {
                if (usersId?.Any() == true || assetsId?.Any() == true)
                    complement += " AND ";

                complement += $"({string.Join(" OR ", ordersStatusType.Select((c, i) => $"o.Status = @Status{i}"))})";
                for (int i = 0; i < ordersStatusType.Count(); ++i)
                    parameters.Add($"Status{i}", ordersStatusType.ElementAt(i).Value, DbType.Int32);
            }
            if (orderType != null)
            {
                if (usersId?.Any() == true || assetsId?.Any() == true || ordersStatusType?.Any() == true)
                    complement += " AND ";

                complement += "o.Type = @Type";
                parameters.Add("Type", orderType.Value, DbType.Int32);
            }
            sql += complement;
        }

        public List<Order> ListOrdersForRankingProfitCalculation(IEnumerable<int> usersId, IEnumerable<int> assetsId)
        {
            if (usersId == null || !usersId.Any())
                return new List<Order>();

            var parameters = new DynamicParameters();
            var complement =  $"({string.Join(" OR ", usersId.Select((c, i) => $"o.UserId = @UserId{i}"))})";
            if (assetsId?.Any() == true)
            {
                complement += $" AND ({string.Join(" OR ", assetsId.Select((c, i) => $"o.AssetId = @AssetId{i}"))})";
                for (int i = 0; i < assetsId.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsId.ElementAt(i), DbType.Int32);
            }
            parameters.Add("CloseStatus", OrderStatusType.Close.Value, DbType.Int32);
            parameters.Add("OpenStatus", OrderStatusType.Open.Value, DbType.Int32);
            parameters.Add("ExecutedStatus", OrderStatusType.Executed.Value, DbType.Int32);
            for (int i = 0; i < usersId.Count(); ++i)
                parameters.Add($"UserId{i}", usersId.ElementAt(i), DbType.Int32);
            
            return Query<Order>(string.Format(SQL_LIST_FOR_CALCULATION, complement), parameters).ToList();
        }

        public List<Order> ListOpenOrdersAndExecutedWithStopLoss(IEnumerable<int> assetsIds)
        {
            if (assetsIds == null || !assetsIds.Any())
                return new List<Order>();

            var parameters = new DynamicParameters();
            var complement = $"({string.Join(" OR ", assetsIds.Select((c, i) => $"o.AssetId = @AssetId{i}"))})";
            parameters.Add("OpenStatus", OrderStatusType.Open.Value, DbType.Int32);
            parameters.Add("ExecutedStatus", OrderStatusType.Executed.Value, DbType.Int32);
            for (int i = 0; i < assetsIds.Count(); ++i)
                parameters.Add($"AssetId{i}", assetsIds.ElementAt(i), DbType.Int32);

            return QueryParentChild<Order, Order, int>(string.Format(SQL_LIST_EXECUTED_WITH_STOP_LOSS_AND_OPEN, complement), c => c.Id, c => c.RelatedOrders, "Id", parameters).ToList();
        }

        public List<Order> ListLastAdvisorsOrdersForAsset(int assetId, IEnumerable<OrderStatusType> orderStatusTypes)
        {
            var complement = "";
            var parameters = new DynamicParameters();
            parameters.Add("AssetId", assetId, DbType.Int32);
            if (orderStatusTypes?.Any() == true)
            {
                complement = $"({string.Join(" OR ", orderStatusTypes.Select((c, i) => $"o.Status = @Status{i}"))})";
                for (int i = 0; i < orderStatusTypes.Count(); ++i)
                    parameters.Add($"Status{i}", orderStatusTypes.ElementAt(i).Value, DbType.Int32);
            }
            return Query<Order>(string.Format(SQL_LAST_ADVISORS_ORDERS_FOR_ASSET, complement), parameters).ToList();
        }

        public Order Get(int orderId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", orderId, DbType.Int32);
            return SelectByParameters<Order>(parameters).SingleOrDefault();
        }

        public List<Order> ListRelatedOrders(IEnumerable<int> ordersId)
        {
            DynamicParameters parameters = new DynamicParameters();
            var complement = "";
            if (ordersId?.Any() == true)
            {
                complement = $" WHERE ({string.Join(" OR ", ordersId.Select((c, i) => $"o.OrderId = @OrderId{i}"))})";
                for (int i = 0; i < ordersId.Count(); ++i)
                    parameters.Add($"OrderId{i}", ordersId.ElementAt(i), DbType.Int32);
            }
            return Query<Order>(string.Format(SQL_LIST, complement), parameters).ToList();
        }

        public List<Order> ListOrdersWithRelated(IEnumerable<int> usersId, IEnumerable<int> assetsId, IEnumerable<OrderStatusType> ordersStatusType, OrderType orderType)
        {
            DynamicParameters parameters;
            var sql = "SELECT o.*, ro.* FROM [Order] o WITH(NOLOCK) LEFT JOIN [Order] ro WITH(NOLOCK) ON ro.OrderId = o.Id ";
            BuildListOrders(ref sql, out parameters, usersId, assetsId, ordersStatusType, orderType);
            return QueryParentChild<Order, Order, int>(sql, c => c.Id, c => c.RelatedOrders, "Id", parameters).ToList();
        }

        public List<Order> ListOrders(IEnumerable<int> usersId, IEnumerable<int> assetsId, IEnumerable<OrderStatusType> ordersStatusType, OrderType orderType)
        {
            DynamicParameters parameters;
            var sql = "SELECT o.* FROM [Order] o WITH(NOLOCK) ";
            BuildListOrders(ref sql, out parameters, usersId, assetsId, ordersStatusType, orderType);
            return Query<Order>(sql, parameters).ToList();
        }

        public Order GetWithRelated(int orderId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", orderId, DbType.Int32);
            return QueryParentChild<Order, Order, int>(SQL_GET_WITH_RELATED, c => c.Id, c => c.RelatedOrders, "Id", parameters).SingleOrDefault();
        }

        public Order GetAnyOrderCreatedByUser(int userId, int usdAssetId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);
            parameters.Add("AssetId", usdAssetId, DbType.Int32);
            return Query<Order>(SQL_ANY_ORDER_CREATED_BY_USER, parameters).FirstOrDefault();
        }
    }
}
