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

        public void UpdateBinanceAssetsValues()
        {
            Dictionary<int, double> currentValues = null;
            List<DomainObjects.Asset.Asset> assets = null;
            Parallel.Invoke(
                () => currentValues = GetAssetsCurrentValuesFromBinanceTicker(),
                () => assets = AssetBusiness.ListAssets()
            );

            UpdateAssetsValues(currentValues);
        }

        private Dictionary<int, double> GetAssetsCurrentValuesFromBinanceTicker()
        {
            var ticker = BinanceBusiness.GetTicker24h();
            var pairs = PairBusiness.ListPairs();
            var usdtPairs = pairs.Where(p => p.QuoteAsset.Code == "USDT");
            var btcPairs = pairs.Where(p => !usdtPairs.Any(usdtPair => usdtPair.BaseAssetId == p.BaseAssetId) && p.QuoteAsset.Code == "BTC");

            var currentValues = new Dictionary<int, double>();

            foreach (var usdtPair in usdtPairs)
            {
                var currentTicker = ticker.First(t => t.Symbol == usdtPair.Symbol);
                currentValues.Add(usdtPair.BaseAssetId, currentTicker.LastPrice);
            }

            foreach (var btcPair in btcPairs)
            {
                var currentTicker = ticker.First(t => t.Symbol == btcPair.Symbol);
                currentValues.Add(btcPair.BaseAssetId, currentTicker.LastPrice * currentValues[btcPair.QuoteAssetId]);
            }

            return currentValues;
        }

        private void UpdateAssetsValues(Dictionary<int, double> currentValues)
        {
            var advisors = AdvisorBusiness.GetAdvisors();
            var advices = AdviceBusiness.List(advisors.Select(c => c.Id));
            var currentDate = Data.GetDateTimeNow();
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);
            
            Parallel.Invoke(() => UpdateAssetCurrentValues(currentDate, currentValues),
                            () => ClosePositionForStopLossAndTargetPriceReached(currentDate, advices, currentValues));
            
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

        private void UpdateAssetCurrentValues(DateTime currentDate, Dictionary<int, double> lastValues)
        {
            var currentValues = new ConcurrentBag<AssetCurrentValue>();
            Parallel.ForEach(lastValues, new ParallelOptions() { MaxDegreeOfParallelism = 7 }, assetToUpdate =>
            {
                currentValues.Add(new AssetCurrentValue()
                {
                    Id = assetToUpdate.Key,
                    UpdateDate = currentDate,
                    CurrentValue = assetToUpdate.Value
                });
            });
            AssetCurrentValueBusiness.UpdateAssetCurrentValues(currentValues);
        }
    }
}
