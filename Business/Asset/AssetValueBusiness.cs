﻿using Auctus.DataAccessInterfaces.Asset;
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
                var filter = new Dictionary<int, DateTime>();
                filter[assetId] = dateTime.Value;
                values = SwingingDoorCompression.Compress(FilterAssetValues(filter).ToDictionary(c => c.Date, c => c.Value))
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
            var advisors = AdvisorBusiness.GetAdvisors();
            Parallel.Invoke(() => assets = AssetBusiness.ListAssets(), () => advices = AdviceBusiness.List(advisors.Select(c => c.Id)), () => assetCurrentValues = AssetCurrentValueBusiness.ListAllAssets());

            var consideredAssetsId = assets.Where(c => advices.Any(a => a.AssetId == c.Id)).Select(c => c.Id).Distinct().ToHashSet();
            var assetValues = new List<AssetValue>();
            foreach (var assetValue in assetResults.Where(c => c.Price.HasValue))
            {
                var asset = assets.FirstOrDefault(c => selectAssetFunc(c, assetValue.Id));
                if (asset != null)
                    assetValues.Add(new AssetValue() { AssetId = asset.Id, Date = currentDate, Value = assetValue.Price.Value, MarketCap = assetValue.MarketCap });
            }

            try
            {
                var assetsToUpdateLastValues = assetCurrentValues.Where(c => assetValues.Any(a => a.AssetId == c.Id));
                var currentValues = new List<AssetCurrentValue>();
                if (assetsToUpdateLastValues.Any())
                {
                    var filter = new List<AssetValueFilter>();
                    foreach (var id in consideredAssetsId)
                    {
                        var assetCurrentValue = assetsToUpdateLastValues.FirstOrDefault(c => c.Id == id);
                        if (assetCurrentValue != null)
                        {
                            if (!assetCurrentValue.Variation24Hours.HasValue && !assetCurrentValue.Variation7Days.HasValue)
                            {
                                //TODO read 30 days of data from coingecko
                            }
                            else
                            {
                                filter.Add(GetFilter(id, currentDate.AddDays(-1)));
                                filter.Add(GetFilter(id, currentDate.AddDays(-7)));
                                filter.Add(GetFilter(id, currentDate.AddDays(-30)));
                            }
                        }
                    }
                    var oldAssetsValues = Filter(filter);

                    foreach (var assetToUpdate in assetsToUpdateLastValues)
                    {
                        var lastAssetValue = assetValues.FirstOrDefault(c => c.AssetId == assetToUpdate.Id);

                        if (consideredAssetsId.Contains(assetToUpdate.Id))
                        {
                            VariantionCalculation(lastAssetValue.Value, currentDate, oldAssetsValues.Where(c => c.AssetId == lastAssetValue.AssetId).OrderByDescending(c => c.Date),
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
            finally
            {
                RunAsync(async () => await InsertManyAssetValuesAsync(assetValues.Where(c => consideredAssetsId.Contains(c.AssetId))));
            }
        }

        private async Task InsertManyAssetValuesAsync(IEnumerable<AssetValue> values)
        {
            var timesToInsert = Math.Ceiling(values.Count() / 50.0);
            for (var i = 0; i < timesToInsert; ++i)
            {
                await Data.InsertManyAsync(values.Skip(i * 50).Take(50));
                await Task.Delay(2000);
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
