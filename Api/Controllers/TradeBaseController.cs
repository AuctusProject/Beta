using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Model.Trade;
using Auctus.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Auctus.DomainObjects.Trade;
using Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers
{
    public class TradeBaseController : BaseController
    {
        protected TradeBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory, hubContext) { }

        protected IActionResult CreateOrder(OrderRequest orderRequest)
        {
            if (orderRequest == null)
                return BadRequest();

            return Ok(OrderBusiness.CreateOrder(orderRequest.AssetId, Auctus.DomainObjects.Trade.OrderType.Get(orderRequest.Type), orderRequest.Quantity, orderRequest.Price, orderRequest.TakeProfit, orderRequest.StopLoss));
        }

        protected IActionResult CloseOrder(int orderId, OrderValueRequest orderValueRequest)
        {
            if (orderValueRequest == null)
                return BadRequest();

            return Ok(OrderBusiness.CloseOrder(orderId, orderValueRequest.Value));
        }

        protected IActionResult CloseAll(CancelCloseAllOrderRequest closeAllOrderRequest)
        {
            if (closeAllOrderRequest == null)
                return BadRequest();

            return Ok(OrderBusiness.CloseAll(closeAllOrderRequest.AssetId));
        }

        protected IActionResult CancelOrder(int orderId)
        {
            return Ok(OrderBusiness.CancelOrder(orderId));
        }

        protected IActionResult CancelAllOpen(CancelCloseAllOrderRequest cancelAllOrderRequest)
        {
            return Ok(OrderBusiness.CancelAllOpen(cancelAllOrderRequest?.AssetId));
        }

        protected IActionResult EditTakeProfit(int orderId, OrderValueRequest orderValueRequest)
        {
            if (orderValueRequest == null)
                return BadRequest();

            return Ok(OrderBusiness.EditTakeProfit(orderId, orderValueRequest.Value));
        }

        protected IActionResult EditStopLoss(int orderId, OrderValueRequest orderValueRequest)
        {
            if (orderValueRequest == null)
                return BadRequest();

            return Ok(OrderBusiness.EditStopLoss(orderId, orderValueRequest.Value));
        }

        protected IActionResult EditOrder(int orderId, EditOrderRequest editOrderRequest)
        {
            if (editOrderRequest == null)
                return BadRequest();

            return Ok(OrderBusiness.EditOrder(orderId, editOrderRequest.Quantity, editOrderRequest.Price, editOrderRequest.TakeProfit, editOrderRequest.StopLoss));
        }
    }
}