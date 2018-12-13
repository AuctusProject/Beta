using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Auctus.Business.Advisor.AdvisorBusiness;

namespace Auctus.Business.Asset
{
    public class AssetCurrentValueBusiness : BaseBusiness<AssetCurrentValue, IAssetCurrentValueData<AssetCurrentValue>>
    {
        public AssetCurrentValueBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<AssetCurrentValue> ListAllAssets(bool enabled, IEnumerable<int> ids = null)
        {
            return Data.ListAllAssets(enabled, ids);
        }

        public List<AssetCurrentValue> ListAssetsFollowedByUser(int userId)
        {
            return Data.ListAssetsFollowedByUser(userId);
        }

        public void UpdateAssetCurrentValues(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            Data.UpdateAssetValue(assetCurrentValues);
        }

        public void UpdateFullAssetCurrentValues(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            Data.UpdateFullAssetValue(assetCurrentValues);
        }

        public void UpdateAssetValue7And30Days(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            Data.UpdateAssetValue7And30Days(assetCurrentValues);
        }

        public double? GetRealCurrentValue(int assetId)
        {
            double? currentValue = null;
            var pairs = PairBusiness.ListPairs(new int[] { assetId });
            if (pairs.Any())
            {
                var usdQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetUSDId);
                if (usdQuote != null)
                    currentValue = BinanceBusiness.GetTicker24h(usdQuote.Symbol)?.LastPrice;
                else
                {
                    var btcQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetBTCId);
                    if (btcQuote != null)
                    {
                        var btcPair = PairBusiness.ListPairs(new int[] { AssetBTCId }, new int[] { AssetUSDId }).FirstOrDefault();
                        if (btcPair != null)
                        {
                            var btcValue = BinanceBusiness.GetTicker24h(btcQuote.Symbol)?.LastPrice;
                            var btcPrice = BinanceBusiness.GetTicker24h(btcPair.Symbol)?.LastPrice;
                            if (btcPrice.HasValue && btcValue.HasValue)
                                currentValue = btcPrice.Value * btcValue.Value;
                        }
                    }
                    else
                    {
                        var ethQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetETHId);
                        if (ethQuote != null)
                        {
                            var ethPair = PairBusiness.ListPairs(new int[] { AssetETHId }, new int[] { AssetUSDId }).FirstOrDefault();
                            if (ethPair != null)
                            {
                                var ethValue = BinanceBusiness.GetTicker24h(ethQuote.Symbol)?.LastPrice;
                                var ethPrice = BinanceBusiness.GetTicker24h(ethPair.Symbol)?.LastPrice;
                                if (ethPrice.HasValue && ethValue.HasValue)
                                    currentValue = ethPrice.Value * ethValue.Value;
                            }
                        }
                    }
                }
            }
            if (!currentValue.HasValue)
            {
                var assetCurrentValue = ListAllAssets(true, new int[] { assetId }).FirstOrDefault();
                if (assetCurrentValue != null && assetCurrentValue.UpdateDate > Data.GetDateTimeNow().AddMinutes(-2))
                    currentValue = assetCurrentValue.CurrentValue;
            }
            return currentValue;
        }
    }
}
