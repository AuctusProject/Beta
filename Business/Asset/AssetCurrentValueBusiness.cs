using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.Model;
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

        public void UpdateAssetValue7And30Days(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            Data.UpdateAssetValue7And30Days(assetCurrentValues);
        }

        public TickerDataModel GetRealCurrentValue(int assetId)
        {
            TickerDataModel currentValue = null;
            var pairs = PairBusiness.ListPairs(new int[] { assetId });
            if (pairs.Any())
            {
                var usdQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetUSDId);
                if (usdQuote != null)
                {
                    var ticker = BinanceBusiness.GetTicker24h(usdQuote.Symbol);
                    if (ticker != null)
                    {
                        currentValue = new TickerDataModel()
                        {
                            AskValue = ticker.AskPrice,
                            BidValue = ticker.BidPrice,
                            CurrentValue = ticker.LastPrice,
                            Variation24Hours = ticker.PriceChangePercent / 100
                        };
                    }
                }
                else
                {
                    var btcQuote = pairs.FirstOrDefault(c => c.QuoteAssetId == AssetBTCId);
                    if (btcQuote != null)
                    {
                        var btcPair = PairBusiness.ListPairs(new int[] { AssetBTCId }, new int[] { AssetUSDId }).FirstOrDefault();
                        if (btcPair != null)
                        {
                            var btcValue = BinanceBusiness.GetTicker24h(btcQuote.Symbol);
                            var btcPrice = BinanceBusiness.GetTicker24h(btcPair.Symbol);
                            if (btcPrice != null && btcValue != null)
                            {
                                currentValue = new TickerDataModel()
                                {
                                    AskValue = btcValue.AskPrice * btcPrice.AskPrice,
                                    BidValue = btcValue.BidPrice * btcPrice.BidPrice,
                                    CurrentValue = btcValue.LastPrice * btcPrice.LastPrice,
                                    Variation24Hours = AssetValueBusiness.GetVariation24h(btcValue.LastPrice, btcValue.PriceChangePercent / 100, btcPrice.LastPrice, btcPrice.PriceChangePercent / 100)
                                };
                            }
                        }
                    }
                }
            }
            if (currentValue == null)
            {
                var assetCurrentValue = ListAllAssets(true, new int[] { assetId }).FirstOrDefault();
                if (assetCurrentValue != null && assetCurrentValue.UpdateDate > Data.GetDateTimeNow().AddMinutes(-2))
                    currentValue = new TickerDataModel()
                    {
                        AskValue = assetCurrentValue.AskValue,
                        BidValue = assetCurrentValue.BidValue,
                        CurrentValue = assetCurrentValue.CurrentValue,
                        Variation24Hours = assetCurrentValue.Variation24Hours
                    };
            }
            return currentValue;
        }
    }
}
