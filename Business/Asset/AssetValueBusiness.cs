using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
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
            dateTime = dateTime ?? Data.GetDateTimeNow().AddDays(-30);
            var asset = AssetBusiness.GetById(assetId);
            var days = Math.Ceiling(Data.GetDateTimeNow().Subtract(dateTime.Value).TotalDays);

            var values = new List<AssetResponse.ValuesResponse>();
            try
            {
                values = CoinGeckoBusiness.GetAssetValues(asset.CoinGeckoId, (int)days);
            }
            catch (ApiException ex)
            {
                var telemetry = new TelemetryClient();
                telemetry.TrackEvent("CoinGeckoBusiness.GetAssetValues");
                telemetry.TrackException(ex);

                var filter = new List<AssetValueFilter>();
                filter.Add(new AssetValueFilter()
                {
                    AssetId = assetId,
                    EndDate = Data.GetDateTimeNow(),
                    StartDate = dateTime.Value
                });
                values = SwingingDoorCompression.Compress(AssetValueBusiness.Filter(filter).ToDictionary(c => c.Date, c => c.Value)).Select(c => new AssetResponse.ValuesResponse()
                {
                    Date = c.Key,
                    Value = c.Value
                }).ToList();
            }
            return values;
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
                values = FilterValueResponse(assetId, dateTime).OrderBy(c => c.Date).ToList();

                if (values.Any())
                    MemoryCache.Set<List<AssetResponse.ValuesResponse>>(cacheKey, values, 20);
            }
            return values;
        }

        public void UpdateCoingeckoAssetsValues()
        {
            UpdateAssetsValues(CoinGeckoBusiness.GetAllCoinsData());
        }

        private void UpdateAssetsValues(IEnumerable<AssetResult> assetResults)
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
                var asset = assets.FirstOrDefault(c => c.CoinGeckoId == assetValue.Id);
                if (asset != null)
                    assetValues.Add(new AssetValue() { AssetId = asset.Id, Date = currentDate, Value = assetValue.Price.Value, MarketCap = assetValue.MarketCap });
            }

            var assetValuesToInsert = assetValues.Where(c => consideredAssetsId.Contains(c.AssetId)).ToList();
            try
            {
                var assetsToUpdateLastValues = assetCurrentValues.Where(c => assetValues.Any(a => a.AssetId == c.Id));
                var currentValues = new List<AssetCurrentValue>();
                if (assetsToUpdateLastValues.Any())
                {
                    var values = new List<AssetValue>();
                    var filter = new List<AssetValueFilter>();
                    foreach (var id in consideredAssetsId)
                    {
                        var assetCurrentValue = assetsToUpdateLastValues.FirstOrDefault(c => c.Id == id);
                        if (assetCurrentValue != null)
                        {
                            if (!assetCurrentValue.Variation24Hours.HasValue && !assetCurrentValue.Variation7Days.HasValue)
                            {
                                var oldValues = CoinGeckoBusiness.GetAssetValues(assetCurrentValue.CoinGeckoId, 31).Select(c => new AssetValue()
                                {
                                    AssetId = assetCurrentValue.Id,
                                    Date = c.Date,
                                    Value = c.Value
                                });
                                values.AddRange(oldValues);
                                assetValuesToInsert.AddRange(oldValues);
                            }
                            else
                            {
                                filter.Add(GetFilter(id, currentDate.AddDays(-1)));
                                filter.Add(GetFilter(id, currentDate.AddDays(-7)));
                                filter.Add(GetFilter(id, currentDate.AddDays(-30)));
                            }
                        }
                    }
                    values.AddRange(FilterAssetValues(filter));

                    foreach (var assetToUpdate in assetsToUpdateLastValues)
                    {
                        var lastAssetValue = assetValues.FirstOrDefault(c => c.AssetId == assetToUpdate.Id);

                        if (consideredAssetsId.Contains(assetToUpdate.Id))
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
            finally
            {
                RunAsync(async () => await InsertManyAssetValuesAsync(assetValuesToInsert));
            }
        }

        private async Task InsertManyAssetValuesAsync(IEnumerable<AssetValue> values)
        {
            var timesToInsert = Math.Ceiling(values.Count() / 90.0);
            for (var i = 0; i < timesToInsert; ++i)
            {
                await Data.InsertManyAsync(values.Skip(i * 90).Take(90));
                await Task.Delay(2000);
            }
        }

        private List<AssetValue> FilterAssetValues(List<AssetValueFilter> filter)
        {
            var result = new List<AssetValue>();
            var timesToQuery = Math.Ceiling(filter.Count / 40.0);
            for (var i = 0; i < timesToQuery; ++i)
            {
                result.AddRange(Filter(filter.Skip(i * 40).Take(40)));
                Task.Delay(1000);
            }

            return result;
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
