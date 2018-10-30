using Auctus.DataAccessInterfaces.Exchange;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.Business.Exchange
{
    public class ExchangeBusiness : BaseBusiness<DomainObjects.Exchange.Exchange, IExchangeData<DomainObjects.Exchange.Exchange>>
    {
        public ExchangeBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<DomainObjects.Exchange.Exchange> ListExchanges()
        {
            string cacheKey = "Exchanges";
            var exchanges = MemoryCache.Get<List<DomainObjects.Exchange.Exchange>>(cacheKey);
            if (exchanges == null)
            {
                exchanges = Data.SelectAll().ToList();
                if (exchanges.Any())
                    MemoryCache.Set<List<DomainObjects.Exchange.Exchange>>(cacheKey, exchanges, 1440);
            }
            return exchanges;
        }

        public DomainObjects.Exchange.Exchange GetById(int id)
        {
            return ListExchanges().FirstOrDefault(c => c.Id == id);
        }
    }
}
