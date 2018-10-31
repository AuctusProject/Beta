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

        public List<AssetCurrentValue> ListAllAssets(IEnumerable<int> ids = null)
        {
            return Data.ListAllAssets(ids);
        }

        public void UpdateAssetCurrentValues(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            Data.UpdateAssetValue(assetCurrentValues);
        }

        public void UpdateAssetValue7And30Days(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            Data.UpdateAssetValue7And30Days(assetCurrentValues);
        }

        public double? GetCurrentValue(int assetId)
        {
            var assetCurrentValue = ListAllAssets(new int[] { assetId });
            if (assetCurrentValue == null || !assetCurrentValue.Any() || assetCurrentValue[0].UpdateDate < Data.GetDateTimeNow().AddHours(-1))
            {
                string assetCoinGeckoId;
                if (assetCurrentValue == null || !assetCurrentValue.Any())
                    assetCoinGeckoId = AssetBusiness.GetById(assetId)?.CoinGeckoId;
                else
                    assetCoinGeckoId = assetCurrentValue[0].CoinGeckoId;

                return CoinGeckoBusiness.GetSimpleCoinData(assetCoinGeckoId)?.Price;
            }
            else
                return assetCurrentValue[0].CurrentValue;
        }

        public List<AssetCurrentValue> ListAssetsValuesForCalculation(IEnumerable<int> assetIds, CalculationMode mode, IEnumerable<Advice> allAdvices, int? selectAssetId = null, int? selectAdvisorId = null)
        {
            var assetCurrentValues = ListAllAssets(assetIds);

            var now = Data.GetDateTimeNow();
            Parallel.ForEach(assetCurrentValues, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, asset =>
            {
                if (asset.UpdateDate < now.AddHours(-1))
                {
                    var assetData = CoinGeckoBusiness.GetFullCoinData(asset.CoinGeckoId);
                    if (assetData != null && assetData.MarketData != null)
                    {
                        asset.UpdateDate = now;
                        if (assetData.MarketData.CurrentPrice != null && assetData.MarketData.CurrentPrice.Value.HasValue)
                            asset.CurrentValue = assetData.MarketData.CurrentPrice.Value.Value;
                        if (assetData.MarketData.PriceChangePercentage24h.HasValue)
                            asset.Variation24Hours = assetData.MarketData.PriceChangePercentage24h.Value / 100.0;
                        if (assetData.MarketData.PriceChangePercentage7d.HasValue)
                            asset.Variation7Days = assetData.MarketData.PriceChangePercentage7d.Value / 100.0;
                        if (assetData.MarketData.PriceChangePercentage30d.HasValue)
                            asset.Variation30Days = assetData.MarketData.PriceChangePercentage30d.Value / 100.0;
                    }
                }
            });
            return assetCurrentValues;
        }
    }
}
