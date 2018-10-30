using Auctus.DataAccessInterfaces.Exchange;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Exchange
{
    public class ExchangeBusiness : BaseBusiness<DomainObjects.Exchange.Exchange, IExchangeData<DomainObjects.Exchange.Exchange>>
    {
        public ExchangeBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

    }
}
