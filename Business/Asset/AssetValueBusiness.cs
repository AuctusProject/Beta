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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.Business.Asset
{
    public class AssetValueBusiness : BaseBusiness<AssetValue, IAssetValueData<AssetValue>>
    {
        public AssetValueBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        //public AssetValue LastAssetValue(int assetId)
        //{
        //    return Filter(new AssetValueFilter[] { GetFilterForCurrentValue(assetId) }).OrderByDescending(c => c.Date).FirstOrDefault();
        //}

        public List<AssetResponse.ValuesResponse> FilterValueResponse(int assetId, DateTime? dateTime)
        {
            dateTime = dateTime ?? Data.GetDateTimeNow().AddDays(-30);
            var asset = AssetBusiness.GetById(assetId);
            var days = Math.Ceiling(Data.GetDateTimeNow().Subtract(dateTime.Value).TotalDays);

            return CoinGeckoBusiness.GetAssetValues(asset.CoinGeckoId, (int)days);
        }

        //public List<AssetValue> Filter(IEnumerable<AssetValueFilter> filter)
        //{
        //    if (filter?.Any() != true)
        //        return new List<AssetValue>();

        //    return Data.Filter(filter);
        //}

        public List<AssetResponse.ValuesResponse> ListAssetValues(int assetId, DateTime? dateTime)
        {
            var baseTime = Data.GetDateTimeNow().AddDays(-30).AddHours(-1);
            if (!dateTime.HasValue || dateTime.Value > baseTime)
                dateTime = baseTime;

            var cacheKey = $"assetValues_{assetId}";
            var values = MemoryCache.Get<List<AssetResponse.ValuesResponse>>(cacheKey);
            if (values == null || values.Min(c => c.Date) > dateTime.Value.AddHours(1))
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
            var currentValues = new Dictionary<int, double>();
            foreach (var assetValue in assetResults.Where(c => c.Price.HasValue))
            {
                var asset = assets.FirstOrDefault(c => c.CoinGeckoId == assetValue.Id);
                if (asset != null)
                {
                    currentValues[asset.Id] = assetValue.Price.Value;
                    assetValues.Add(new AssetValue() { AssetId = asset.Id, Date = currentDate, Value = assetValue.Price.Value, MarketCap = assetValue.MarketCap });
                }
            }

            var assetsToUpdateLastValues = assetCurrentValues.Where(c => currentValues.ContainsKey(c.Id)).ToList();
            if (assetsToUpdateLastValues.Any())
            {
                Parallel.Invoke(() => UpdateAssetCurrentValues(currentDate, currentValues, consideredAssetsId, assetsToUpdateLastValues),
                                () => ClosePositionForStopLossAndTargetPriceReached(currentDate, advices, currentValues));
            }
        }

        private void ClosePositionForStopLossAndTargetPriceReached(DateTime currentDate, List<Advice> advices, Dictionary<int, double> lastValues)
        {
            var newAutomatedAdvices = new List<Advice>();
            var consideredAdvices = advices.Where(c => lastValues.ContainsKey(c.AssetId)).GroupBy(c => new { c.AssetId, c.AdvisorId }).
                Select(c => new { c.Key.AssetId, c.Key.AdvisorId, CreationDate = c.Max(a => a.CreationDate) }).ToList();
            foreach (var adviceKey in consideredAdvices)
            {
                var advice = advices.FirstOrDefault(c => c.AssetId == adviceKey.AssetId && c.AdvisorId == adviceKey.AdvisorId && c.CreationDate == adviceKey.CreationDate &&
                    (c.StopLoss.HasValue || c.TargetPrice.HasValue));
                if (advice != null)
                {
                    if (advice.StopLoss.HasValue)
                    {
                        if (advice.AdviceType == AdviceType.Buy && lastValues[advice.AssetId] <= advice.StopLoss.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, lastValues[advice.AssetId], AdviceOperationType.StopLoss));
                        else if (advice.AdviceType == AdviceType.Sell && lastValues[advice.AssetId] >= advice.StopLoss.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, lastValues[advice.AssetId], AdviceOperationType.StopLoss));
                    }
                    if (advice.TargetPrice.HasValue)
                    {
                        if (advice.AdviceType == AdviceType.Buy && lastValues[advice.AssetId] >= advice.TargetPrice.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, lastValues[advice.AssetId], AdviceOperationType.TargetPrice));
                        else if (advice.AdviceType == AdviceType.Sell && lastValues[advice.AssetId] <= advice.TargetPrice.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, lastValues[advice.AssetId], AdviceOperationType.TargetPrice));
                    }
                }
            }
            AdviceBusiness.InsertAutomatedClosePositionAdvices(newAutomatedAdvices);
        }

        private Advice CreateAutomatedAdvice(DateTime currentDate, int assetId, int advisorId, double currentValue, AdviceOperationType operationType)
        {
            return new Advice()
            {
                AssetId = assetId,
                AdvisorId = advisorId,
                AssetValue = currentValue,
                CreationDate = currentDate,
                Type = AdviceType.ClosePosition.Value,
                OperationType = operationType.Value
            };
        }

        private void UpdateAssetCurrentValues(DateTime currentDate, Dictionary<int, double> lastValues, HashSet<int> consideredAssetsId, 
            List<AssetCurrentValue> assetsToUpdateLastValues)
        {
            var currentValues = new ConcurrentBag<AssetCurrentValue>();
            Parallel.ForEach(assetsToUpdateLastValues, new ParallelOptions() { MaxDegreeOfParallelism = 7 }, assetToUpdate =>
            {
                var lastAssetValue = lastValues[assetToUpdate.Id];

                if (consideredAssetsId.Contains(assetToUpdate.Id))
                {
                    var assetData = CoinGeckoBusiness.GetFullCoinData(assetToUpdate.CoinGeckoId);
                    if (assetData != null && assetData.MarketData != null)
                    {
                        currentValues.Add(new AssetCurrentValue()
                        {
                            Id = assetToUpdate.Id,
                            UpdateDate = currentDate,
                            CurrentValue = lastAssetValue,
                            Variation24Hours = assetData.MarketData.PriceChangePercentage24h.HasValue ? assetData.MarketData.PriceChangePercentage24h.Value / 100.0 : (double?)null,
                            Variation7Days = assetData.MarketData.PriceChangePercentage7d.HasValue ? assetData.MarketData.PriceChangePercentage7d.Value / 100.0 : (double?)null,
                            Variation30Days = assetData.MarketData.PriceChangePercentage30d.HasValue ? assetData.MarketData.PriceChangePercentage30d.Value / 100.0 : (double?)null
                        });
                    }
                    else
                    {
                        currentValues.Add(new AssetCurrentValue()
                        {
                            Id = assetToUpdate.Id,
                            UpdateDate = currentDate,
                            CurrentValue = lastAssetValue
                        });
                    }
                }
                else
                {
                    currentValues.Add(new AssetCurrentValue()
                    {
                        Id = assetToUpdate.Id,
                        UpdateDate = currentDate,
                        CurrentValue = lastAssetValue
                    });
                }
            });
            AssetCurrentValueBusiness.UpdateAssetCurrentValues(currentValues);
        }

        //private void InsertManyAssetValuesAsync(IEnumerable<AssetValue> values)
        //{
        //    var timesToInsert = Math.Ceiling(values.Count() / 90.0);
        //    for (var i = 0; i < timesToInsert; ++i)
        //    {
        //        Data.InsertManyAsync(values.Skip(i * 90).Take(90)).Wait();
        //        Task.Delay(2000).Wait();
        //    }
        //}

        //private List<AssetValue> FilterAssetValues(List<AssetValueFilter> filter)
        //{
        //    var result = new List<AssetValue>();
        //    var timesToQuery = Math.Floor(filter.Count / 50.0);
        //    for (var i = 0; i < timesToQuery; ++i)
        //    {
        //        result.AddRange(Filter(filter.Skip(i * 50).Take(50)));
        //        Task.Delay(200).Wait();
        //    }

        //    return result;
        //}

        //public void VariantionCalculation(double currentValue, DateTime currentDate, IEnumerable<AssetValue> values, 
        //    out double? variation24h, out double? variation7d, out double? variation30d)
        //{
        //    var vl30d = values.Where(c => c.Date <= currentDate.AddDays(-30) && c.Date > currentDate.AddDays(-31));
        //    var vl7d = values.Where(c => c.Date <= currentDate.AddDays(-7) && c.Date > currentDate.AddDays(-8));
        //    var vl24h = values.Where(c => c.Date <= currentDate.AddDays(-1) && c.Date > currentDate.AddDays(-2));
        //    variation24h = vl24h.Any() ? (currentValue / vl24h.First().Value) - 1 : (double?)null;
        //    variation7d = vl7d.Any() ? (currentValue / vl7d.First().Value) - 1 : (double?)null;
        //    variation30d = vl30d.Any() ? (currentValue / vl30d.First().Value) - 1 : (double?)null;
        //}

        //public AssetValueFilter GetFilter(int assetId, DateTime endDate)
        //{
        //    return new AssetValueFilter()
        //    {
        //        AssetId = assetId,
        //        StartDate = endDate.AddHours(-1),
        //        EndDate = endDate
        //    };
        //}

        //public AssetValueFilter GetFilterForCurrentValue(int assetId)
        //{
        //    return GetFilter(assetId, Data.GetDateTimeNow());
        //}

        //public AssetValueFilter GetFilterFor24hValue(int assetId)
        //{
        //    return GetFilter(assetId, Data.GetDateTimeNow().AddDays(-1));
        //}

        //public AssetValueFilter GetFilterFor7dValue(int assetId)
        //{
        //    return GetFilter(assetId, Data.GetDateTimeNow().AddDays(-7));
        //}

        //public AssetValueFilter GetFilterFor30dValue(int assetId)
        //{
        //    return GetFilter(assetId, Data.GetDateTimeNow().AddDays(-30));
        //}
    }
}
