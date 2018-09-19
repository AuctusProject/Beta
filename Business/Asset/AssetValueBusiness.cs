using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.Util;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.Business.Asset
{
    public class AssetValueBusiness : BaseBusiness<AssetValue, IAssetValueData<AssetValue>>
    {
        public AssetValueBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public AssetValue LastAssetValue(int assetId)
        {
            return Data.GetLastValue(assetId);
        }

        public List<AssetValue> FilterAssetValues(Dictionary<int, DateTime> assetsMap)
        {
            return Data.FilterAssetValues(assetsMap);
        }

        public void UpdateCoinmarketcapAssetsValues()
        {
            UpdateAssetsValues(CoinMarketcapBusiness.GetAllCoinsData(), IsCoinmarketcapAsset);
        }

        private bool IsCoinmarketcapAsset(DomainObjects.Asset.Asset asset, string key)
        {
            return asset.CoinMarketCapId == Convert.ToInt32(key);
        }

        public void UpdateCoingeckoAssetsValues()
        {
            UpdateAssetsValues(CoinGeckoBusiness.GetAllCoinsData(), IsCoingeckoAsset);
        }

        private bool IsCoingeckoAsset(DomainObjects.Asset.Asset asset, string key)
        {
            return asset.CoinGeckoId == key;
        }

        private void UpdateAssetsValues(IEnumerable<AssetResult> assetResults, Func<DomainObjects.Asset.Asset, string, bool> selectAssetFunc)
        {
            var currentDate = Data.GetDateTimeNow();
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);

            List<DomainObjects.Asset.Asset> assets = null;
            List<Advice> advices = null;
            List<AssetCurrentValue> assetCurrentValues = null;
            Parallel.Invoke(() => assets = AssetBusiness.ListAssets(), () => advices = AdviceBusiness.ListAllCached(), () => assetCurrentValues = AssetCurrentValueBusiness.ListAllAssets());

            var assetValues = new List<AssetValue>();
            foreach (var assetValue in assetResults.Where(c => c.Price.HasValue))
            {
                var asset = assets.FirstOrDefault(c => selectAssetFunc(c, assetValue.Id));
                if (asset != null)
                    assetValues.Add(new AssetValue() { AssetId = asset.Id, Date = currentDate, Value = assetValue.Price.Value, MarketCap = assetValue.MarketCap });
            }
            Data.InsertManyAsync(assetValues);

            var baseDate = currentDate.AddDays(-30).AddHours(-4);
            var assetsToUpdateLastValues = assetCurrentValues.Where(c => advices.Any(a => a.AssetId == c.Id) && assetValues.Any(a => a.AssetId == c.Id)).ToDictionary(c => c.Id, c => baseDate);
            var currentValues = new List<AssetCurrentValue>();
            if (assetsToUpdateLastValues.Any())
            {
                var values = FilterAssetValues(assetsToUpdateLastValues);
                foreach (var assetToUpdate in assetsToUpdateLastValues)
                {
                    var lastAssetValue = assetValues.FirstOrDefault(c => c.AssetId == assetToUpdate.Key);
                    if (lastAssetValue != null)
                    {
                        VariantionCalculation(lastAssetValue.Value, currentDate, values.Where(c => c.AssetId == lastAssetValue.AssetId).OrderByDescending(c => c.Date),
                            out double? variation24h, out double? variation7d, out double? variation30d);

                        currentValues.Add(new AssetCurrentValue()
                        {
                            Id = lastAssetValue.AssetId,
                            UpdateDate = currentDate,
                            CurrentValue = lastAssetValue.Value,
                            Variation24Hours = variation24h,
                            Variation7Days = variation7d,
                            Variation30Days = variation30d
                        });
                    }
                }
                if (currentValues.Any())
                {
                    using (var transaction = TransactionalDapperCommand)
                    {
                        foreach (var value in currentValues)
                            transaction.Update(value);

                        transaction.Commit();
                    }
                }
            }
        }

        public void VariantionCalculation(double currentValue, DateTime currentDate, IEnumerable<AssetValue> values, 
            out double? variation24h, out double? variation7d, out double? variation30d)
        {
            var vl30d = values.Where(c => c.Date <= currentDate.AddDays(-30) && c.Date > currentDate.AddDays(-31));
            var vl7d = values.Where(c => c.Date <= currentDate.AddDays(-7) && c.Date > currentDate.AddDays(-8));
            var vl24h = values.Where(c => c.Date <= currentDate.AddDays(-1) && c.Date > currentDate.AddDays(-2));
            variation24h = vl24h.Any() ? (currentValue / vl24h.First().Value) - 1 : (double?)null;
            variation7d = vl7d.Any() ? (currentValue / vl7d.First().Value) - 1 : (double?)null;
            variation30d = vl30d.Any() ? (currentValue / vl30d.First().Value) - 1 : (double?)null;
        }
    }
}
