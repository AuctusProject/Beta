using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
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
    public class PairBusiness : BaseBusiness<Pair, IPairData<Pair>>
    {
        public PairBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<Pair> ListPairs(IEnumerable<int> baseAssetIds = null, IEnumerable<int> baseQuoteIds = null)
        {
            string cacheKey = "Pairs";
            var pairs = MemoryCache.Get<List<Pair>>(cacheKey);
            if (pairs == null)
            {
                pairs = Data.ListEnabled().ToList();
                pairs.ForEach(c => c.Exchange = ExchangeBusiness.GetById(c.BaseAssetId));
                if (pairs.Any())
                    MemoryCache.Set<List<Pair>>(cacheKey, pairs, 720);
            }
            return baseAssetIds == null && baseQuoteIds == null ? pairs : baseAssetIds != null && baseQuoteIds != null ? 
                pairs.Where(c => baseAssetIds.Contains(c.BaseAssetId) && baseQuoteIds.Contains(c.QuoteAssetId)).ToList() : baseAssetIds != null ?
                pairs.Where(c => baseAssetIds.Contains(c.BaseAssetId)).ToList() : pairs.Where(c => baseQuoteIds.Contains(c.QuoteAssetId)).ToList();
        }
    }
}
