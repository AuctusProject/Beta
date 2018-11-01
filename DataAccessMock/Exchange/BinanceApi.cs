using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Exchange
{
    public class BinanceApi : IBinanceApi
    {
        public BinanceTicker[] GetTicker24h()
        {
            throw new NotImplementedException();
        }

        public BinanceTicker GetTicker24h(string symbol)
        {
            return null;
        }

        public BinanceKline GetKline30d(string symbol)
        {
            throw new NotImplementedException();
        }

        public BinanceKline GetKline7d(string symbol)
        {
            throw new NotImplementedException();
        }
    }
}
