using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Exchange;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Exchange
{
    public class ExchangeData : BaseSql<DomainObjects.Exchange.Exchange>, IExchangeData<DomainObjects.Exchange.Exchange>
    {
        public override string TableName => "Exchange";

        public ExchangeData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
