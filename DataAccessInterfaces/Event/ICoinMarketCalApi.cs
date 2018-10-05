using Auctus.DomainObjects.Event;
using System;
using System.Collections.Generic;
using System.Text;
using static Auctus.DomainObjects.Event.CoinMarketCalResult;

namespace Auctus.DataAccessInterfaces.Event
{
    public interface ICoinMarketCalApi
    {
        List<Coin> ListCoins();
        List<Category> ListCategories();
        CoinMarketCalResult ListEvents(int page = 1, int limit = 100, DateTime? startDate = null, DateTime? endDate = null);
    }
}
