
using Auctus.DataAccess.Account;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auctus.DataAccess.Exchanges;

namespace Auctus.Business.Account
{
    public class ExchangeApiAccessBusiness : BaseBusiness<ExchangeApiAccess, ExchangeApiAccessData>
    {
        public ExchangeApiAccessBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }
    }
}
