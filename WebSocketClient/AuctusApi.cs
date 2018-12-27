using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Exchange;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketClient
{
    public class AuctusApi : ApiBase
    {
        private const string EXECUTE_ORDERS_JOB = "api/v1/jobs/assets/binance/executeOrders";

        public AuctusApi(String bearerToken, string baseUrl) : base(baseUrl)
        {
            BearerToken = bearerToken;
        }

        public void PostExecuteOrders(BinanceWebSocketTicker[] ticker)
        {
            PostWithRetry(EXECUTE_ORDERS_JOB, ticker);
        }
    }
}
