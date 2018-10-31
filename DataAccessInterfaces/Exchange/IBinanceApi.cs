using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Exchange
{
    public interface IBinanceApi
    {
        BinanceTicker[] GetTicker24h();
        BinanceKline GetKline7d(string symbol);
        BinanceKline GetKline30d(string symbol);
        BinanceTicker GetTicker24h(string symbol);
    }
}
