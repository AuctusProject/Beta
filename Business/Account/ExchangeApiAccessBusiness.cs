
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
using Microsoft.Extensions.Configuration;

namespace Auctus.Business.Account
{
    public class ExchangeApiAccessBusiness : BaseBusiness<ExchangeApiAccess, IExchangeApiAccessData<ExchangeApiAccess>>
    {
        public ExchangeApiAccessBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, loggerFactory, cache, email, ip) { }
    }
}
