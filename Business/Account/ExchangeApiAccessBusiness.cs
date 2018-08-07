
using Auctus.DataAccess.Account;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auctus.DataAccess.Exchanges;
using Auctus.DataAccessInterfaces.Account;

namespace Auctus.Business.Account
{
    public class ExchangeApiAccessBusiness : BaseBusiness<ExchangeApiAccess, IExchangeApiAccessData<ExchangeApiAccess>>
    {
        public ExchangeApiAccessBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }
    }
}
