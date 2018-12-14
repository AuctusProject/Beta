using Api.Hubs;
using Api.Model.Trade;
using Auctus.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/trades/")]
    [EnableCors("Default")]
    public class TradeV1Controller : TradeBaseController
    {
        public TradeV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory, hubContext) { }

        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult CreateOrder([FromBody]OrderRequest orderRequest)
        {
            return base.CreateOrder(orderRequest);
        }

        [Route("{orderId}/close")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult CloseOrder([FromRoute]int orderId, [FromBody]OrderValueRequest orderValueRequest)
        {
            return base.CloseOrder(orderId, orderValueRequest);
        }

        [Route("{orderId}/cancel")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult CancelOrder([FromRoute]int orderId)
        {
            return base.CancelOrder(orderId);
        }

        [Route("{orderId}/stop_loss")]
        [HttpPut]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult EditStopLoss([FromRoute]int orderId, [FromBody]OrderValueRequest orderValueRequest)
        {
            return base.EditStopLoss(orderId, orderValueRequest);
        }

        [Route("{orderId}/take_profit")]
        [HttpPut]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult EditTakeProfit([FromRoute]int orderId, [FromBody]OrderValueRequest orderValueRequest)
        {
            return base.EditTakeProfit(orderId, orderValueRequest);
        }

        [Route("{orderId}")]
        [HttpPut]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult EditOrder([FromRoute]int orderId, [FromBody]EditOrderRequest editOrderRequest)
        {
            return base.EditOrder(orderId, editOrderRequest);
        }

        [Route("cancel_all")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult CancelAllOpen([FromBody]CancelCloseAllOrderRequest cancelAllOrderRequest)
        {
            return base.CancelAllOpen(cancelAllOrderRequest);
        }

        [Route("close_all")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult CloseAll([FromBody]CancelCloseAllOrderRequest closeAllOrderRequest)
        {
            return base.CloseAll(closeAllOrderRequest);
        }

        [Route("followed_trades")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListFollowedTrades()
        {
            return base.ListFollowedTrades();
        }
    }
}
