using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.Model;
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
            return Filter(new AssetValueFilter[] { GetFilterForCurrentValue(assetId) }).OrderByDescending(c => c.Date).FirstOrDefault();
        }

        public List<AssetResponse.ValuesResponse> FilterValueResponse(int assetId, DateTime? dateTime)
        {
            var asset = AssetBusiness.GetById(assetId);
            var days = Math.Ceiling((DateTime.UtcNow - (dateTime ?? DateTime.UtcNow.AddDays(-30))).TotalDays);

            return CoinGeckoBusiness.GetAssetValues(asset.CoinGeckoId, (int)days);
        }

        public List<AssetValue> Filter(IEnumerable<AssetValueFilter> filter)
        {
            if (filter?.Any() != true)
                return new List<AssetValue>();

            return Data.Filter(filter);
        }

        public List<AssetResponse.ValuesResponse> ListAssetValues(int assetId, DateTime? dateTime)
        {
            var baseTime = Data.GetDateTimeNow().AddDays(-30).AddHours(-4);
            if (!dateTime.HasValue || dateTime.Value > baseTime)
                dateTime = baseTime;

            var cacheKey = $"assetValues_{assetId}";
            var values = MemoryCache.Get<List<AssetResponse.ValuesResponse>>(cacheKey);
            if (values == null || values.Min(c => c.Date) > dateTime.Value.AddHours(2))
            {
                values = SwingingDoorCompression.Compress(FilterValueResponse(assetId, dateTime).ToDictionary(c => c.Date, c => c.Value))
                                        .Select(c => new AssetResponse.ValuesResponse() { Date = c.Key, Value = c.Value }).OrderBy(c => c.Date).ToList();

                if (values.Any())
                    MemoryCache.Set<List<AssetResponse.ValuesResponse>>(cacheKey, values, 30);
            }
            return values;
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
            RunAsync(async () => await InsertManyAssetValuesAsync(assetValues));
            
            var assetsToUpdateLastValues = assetCurrentValues.Where(c => assetValues.Any(a => a.AssetId == c.Id));
            var currentValues = new List<AssetCurrentValue>();
            if (assetsToUpdateLastValues.Any())
            {
                var assetsWithAdvices = assetsToUpdateLastValues.Where(c => advices.Any(a => a.AssetId == c.Id)).Select(c => c.Id).Distinct().ToHashSet();
                var filter = new List<AssetValueFilter>();
                foreach(var asset in assetsWithAdvices)
                {
                    filter.Add(GetFilter(asset, currentDate.AddDays(-1)));
                    filter.Add(GetFilter(asset, currentDate.AddDays(-7)));
                    filter.Add(GetFilter(asset, currentDate.AddDays(-30)));
                }
                var values = Filter(filter);
                foreach (var assetToUpdate in assetsToUpdateLastValues)
                {
                    var lastAssetValue = assetValues.FirstOrDefault(c => c.AssetId == assetToUpdate.Id);

                    if (assetsWithAdvices.Contains(assetToUpdate.Id))
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
                    else
                    {
                        currentValues.Add(new AssetCurrentValue()
                        {
                            Id = lastAssetValue.AssetId,
                            UpdateDate = currentDate,
                            CurrentValue = lastAssetValue.Value
                        });
                    }
                }
                AssetCurrentValueBusiness.UpdateAssetCurrentValues(currentValues);
            }
        }

        private async Task InsertManyAssetValuesAsync(List<AssetValue> values)
        {
            var timesToInsert = Math.Ceiling(values.Count / 100.0);
            for (var i = 0; i < timesToInsert; ++i)
            {
                await Data.InsertManyAsync(values.Skip(i * 100).Take(100));
                await Task.Delay(1001);
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

        public AssetValueFilter GetFilter(int assetId, DateTime endDate)
        {
            return new AssetValueFilter()
            {
                AssetId = assetId,
                StartDate = endDate.AddHours(-4),
                EndDate = endDate
            };
        }

        public AssetValueFilter GetFilterForCurrentValue(int assetId)
        {
            return GetFilter(assetId, Data.GetDateTimeNow());
        }

        public AssetValueFilter GetFilterFor24hValue(int assetId)
        {
            return GetFilter(assetId, Data.GetDateTimeNow().AddDays(-1));
        }

        public AssetValueFilter GetFilterFor7dValue(int assetId)
        {
            return GetFilter(assetId, Data.GetDateTimeNow().AddDays(-7));
        }

        public AssetValueFilter GetFilterFor30dValue(int assetId)
        {
            return GetFilter(assetId, Data.GetDateTimeNow().AddDays(-30));
        }
    }
}
