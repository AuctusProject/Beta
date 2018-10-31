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
using static Auctus.Model.AssetResponse;

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
                pairs.ForEach(c => 
                {
                    c.Exchange = ExchangeBusiness.GetById(c.ExchangeId);
                    c.BaseAsset = AssetBusiness.GetById(c.BaseAssetId);
                    c.QuoteAsset = AssetBusiness.GetById(c.QuoteAssetId);
                });
                if (pairs.Any())
                    MemoryCache.Set<List<Pair>>(cacheKey, pairs, 720);
            }
            return baseAssetIds == null && baseQuoteIds == null ? pairs : baseAssetIds != null && baseQuoteIds != null ? 
                pairs.Where(c => baseAssetIds.Contains(c.BaseAssetId) && baseQuoteIds.Contains(c.QuoteAssetId)).ToList() : baseAssetIds != null ?
                pairs.Where(c => baseAssetIds.Contains(c.BaseAssetId)).ToList() : pairs.Where(c => baseQuoteIds.Contains(c.QuoteAssetId)).ToList();
        }

        public PairResponse GetBaseQuotePair(int assetId)
        {
            var pairs = ListPairs(new int[] { assetId });
            if (!pairs.Any())
                return null;

            var preffix = "$";
            var usdQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetUSDId);
            if (usdQuote != null)
            {
                return new PairResponse()
                {
                    Symbol = usdQuote.Symbol,
                    Preffix = preffix
                };
            }
            else
            {
                var btcQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetBTCId);
                if (btcQuote != null)
                {
                    var btcUSD = ListPairs(new int[] { AssetBTCId }, new int[] { AssetUSDId }).First();
                    return new PairResponse()
                    {
                        Symbol = btcQuote.Symbol,
                        MultipliedSymbol = btcUSD.Symbol,
                        Preffix = preffix
                    };
                }
                else
                {
                    var ethQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetETHId);
                    if (ethQuote != null)
                    {
                        var ethUSD = ListPairs(new int[] { AssetETHId }, new int[] { AssetUSDId }).First();
                        return new PairResponse()
                        {
                            Symbol = ethQuote.Symbol,
                            MultipliedSymbol = ethUSD.Symbol,
                            Preffix = preffix
                        };
                    }
                    else
                        return null;
                }
            }
        }
    }
}
