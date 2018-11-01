using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Exchange
{
    public class ExchangeData : BaseData<DomainObjects.Exchange.Exchange>, IExchangeData<DomainObjects.Exchange.Exchange>
    {
        public override IEnumerable<DomainObjects.Exchange.Exchange> SelectAll()
        {
            return new List<DomainObjects.Exchange.Exchange>()
            {
                new DomainObjects.Exchange.Exchange()
                {
                    Id = 1,
                    Name = "TesterExchange"
                }
            };
        }
    }
}
