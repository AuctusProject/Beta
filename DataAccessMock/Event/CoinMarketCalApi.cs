using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Event
{
    public class CoinMarketCalApi : ICoinMarketCalApi
    {
        public List<CoinMarketCalResult.Category> ListCategories()
        {
            throw new NotImplementedException();
        }

        public List<CoinMarketCalResult.Coin> ListCoins()
        {
            throw new NotImplementedException();
        }

        public CoinMarketCalResult ListEvents(int page = 1, int limit = 100, DateTime? startDate = null, DateTime? endDate = null)
        {
            throw new NotImplementedException();
        }
    }
}
