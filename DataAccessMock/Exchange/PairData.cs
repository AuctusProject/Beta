using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Exchange
{
    public class PairData : BaseData<Pair>, IPairData<Pair>
    {
        public IEnumerable<Pair> ListEnabled()
        {
            return new List<Pair>()
            {
                new Pair()
                {
                    BaseAssetId = 1,
                    QuoteAssetId = 241,
                    Enabled = true
                },
                new Pair()
                {
                    BaseAssetId = 2,
                    QuoteAssetId = 241,
                    Enabled = true
                },
                new Pair()
                {
                    BaseAssetId = 3,
                    QuoteAssetId = 241,
                    Enabled = true
                },
                new Pair()
                {
                    BaseAssetId = 4,
                    QuoteAssetId = 241,
                    Enabled = true
                }
            };
        }
    }
}
