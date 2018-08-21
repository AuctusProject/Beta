using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Exchange
{
    public class CoinGeckoApi : ICoinGeckoApi
    {
        public IEnumerable<AssetResult> GetAllCoinsData()
        {
            throw new NotImplementedException();
        }
    }
}
