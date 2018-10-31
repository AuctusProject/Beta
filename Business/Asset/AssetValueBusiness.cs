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
            Dictionary<int, Tuple<double, double>> currentValues = null;
            List<DomainObjects.Asset.Asset> assets = null;
            Parallel.Invoke(
                () => currentValues = GetAssetsCurrentValuesAndVariationFromBinanceTicker(),
                () => assets = AssetBusiness.ListAssets()
            );

            UpdateAssetsValues(currentValues);
        }

        private Dictionary<int, Tuple<double, double>> GetAssetsCurrentValuesAndVariationFromBinanceTicker()
        {
            var ticker = BinanceBusiness.GetTicker24h();
            var pairs = PairBusiness.ListPairs();
            var usdtPairs = pairs.Where(p => p.QuoteAsset.Code == "USDT");
            var btcPairs = pairs.Where(p => !usdtPairs.Any(usdtPair => usdtPair.BaseAssetId == p.BaseAssetId) && p.QuoteAsset.Code == "BTC");

            var currentValues = new Dictionary<int, Tuple<double, double>>();

            foreach (var usdtPair in usdtPairs)
            {
                var currentTicker = ticker.First(t => t.Symbol == usdtPair.Symbol);
                currentValues.Add(usdtPair.BaseAssetId, new Tuple<double, double>(currentTicker.LastPrice, currentTicker.PriceChangePercent));
            }

            foreach (var btcPair in btcPairs)
            {
                var currentTicker = ticker.First(t => t.Symbol == btcPair.Symbol);
                var variation24h = ((1 * (currentTicker.PriceChangePercent / 100.0 + 1) * (1 + currentValues[btcPair.QuoteAssetId].Item2 / 100.0)) - 1) * 100;
                currentValues.Add(btcPair.BaseAssetId, new Tuple<double, double>(currentTicker.LastPrice * currentValues[btcPair.QuoteAssetId].Item1, variation24h));
            }

            return currentValues;
        }

        public void UpdateBinanceAssetsValues7dAnd30d()
        {
            ConcurrentDictionary<int, Tuple<double?, double?>> currentValues = GetAssets7dAnd30dVariationFromBinanceTicker();
            UpdateAsset7dAnd30dValues(currentValues);
        }

        private ConcurrentDictionary<int, Tuple<double?, double?>> GetAssets7dAnd30dVariationFromBinanceTicker()
        {
            var pairs = PairBusiness.ListPairs();
            var usdtPairs = pairs.Where(p => p.QuoteAsset.Code == "USDT");
            var btcPairs = pairs.Where(p => !usdtPairs.Any(usdtPair => usdtPair.BaseAssetId == p.BaseAssetId) && p.QuoteAsset.Code == "BTC");

            var currentValues = new ConcurrentDictionary<int, Tuple<double?, double?>>();

            Parallel.ForEach(usdtPairs,
                new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                (usdtPair) =>
                {
                    var variation = Get7dAnd30dVariation(usdtPair.Symbol);
                    currentValues.TryAdd(usdtPair.BaseAssetId, variation);
                });

            Parallel.ForEach(btcPairs,
                new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                (btcPair) =>
                {
                    var variation = Get7dAnd30dVariation(btcPair.Symbol);
                    var totalVariation7d = GetVariationWithMultiplierQuote(variation.Item1 , currentValues[btcPair.QuoteAssetId].Item2);
                    var totalVariation30d = GetVariationWithMultiplierQuote(variation.Item2, currentValues[btcPair.QuoteAssetId].Item2);
                    currentValues.TryAdd(btcPair.BaseAssetId, new Tuple<double?, double?>(totalVariation7d, totalVariation30d));
                });

            return currentValues;
        }

        private static double? GetVariationWithMultiplierQuote(double? variationBase, double? variationQuote)
        {
            return ((1 * (variationBase / 100.0 + 1) * (1 + variationQuote / 100.0)) - 1) * 100;
        }

        private Tuple<double?, double?> Get7dAnd30dVariation(string symbol)
        {
            var kline7d = BinanceBusiness.GetKline7d(symbol);
            var kline30d = BinanceBusiness.GetKline30d(symbol);
            var variation7d = GetVariationFromKline(kline7d);
            var variation30d = GetVariationFromKline(kline30d);
            return new Tuple<double?, double?>(variation7d, variation30d);
        }

        private static double? GetVariationFromKline(BinanceKline kline)
        {
            if (kline == null)
                return null;
            return (kline.Close - kline.Open) * 100.0 / kline.Open;
        }

        private void UpdateAssetsValues(Dictionary<int, Tuple<double, double>> currentValues)
        {
            var advisors = AdvisorBusiness.GetAdvisors();
            var advices = AdviceBusiness.List(advisors.Select(c => c.Id));
            var currentDate = Data.GetDateTimeNow();
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);

            Parallel.Invoke(() => UpdateAssetCurrentValues(currentDate, currentValues),
                            () => ClosePositionForStopLossAndTargetPriceReached(currentDate, advices, currentValues));

        }

        private void ClosePositionForStopLossAndTargetPriceReached(DateTime currentDate, List<Advice> advices, Dictionary<int, Tuple<double, double>> lastValues)
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
                    var assetLastValue = lastValues[advice.AssetId].Item1;
                    if (advice.StopLoss.HasValue)
                    {
                        if (advice.AdviceType == AdviceType.Buy && assetLastValue <= advice.StopLoss.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, assetLastValue, AdviceOperationType.StopLoss));
                        else if (advice.AdviceType == AdviceType.Sell && assetLastValue >= advice.StopLoss.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, assetLastValue, AdviceOperationType.StopLoss));
                    }
                    if (advice.TargetPrice.HasValue)
                    {
                        if (advice.AdviceType == AdviceType.Buy && assetLastValue >= advice.TargetPrice.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, assetLastValue, AdviceOperationType.TargetPrice));
                        else if (advice.AdviceType == AdviceType.Sell && assetLastValue <= advice.TargetPrice.Value)
                            newAutomatedAdvices.Add(CreateAutomatedAdvice(currentDate, advice.AssetId, advice.AdvisorId, assetLastValue, AdviceOperationType.TargetPrice));
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

        private void UpdateAssetCurrentValues(DateTime currentDate, Dictionary<int, Tuple<double, double>> lastValues)
        {
            var currentValues = new ConcurrentBag<AssetCurrentValue>();
            Parallel.ForEach(lastValues, new ParallelOptions() { MaxDegreeOfParallelism = 7 }, assetToUpdate =>
            {
                currentValues.Add(new AssetCurrentValue()
                {
                    Id = assetToUpdate.Key,
                    UpdateDate = currentDate,
                    CurrentValue = assetToUpdate.Value.Item1,
                    Variation24Hours = assetToUpdate.Value.Item2
                });
            });
            AssetCurrentValueBusiness.UpdateAssetCurrentValues(currentValues);
        }

        private void UpdateAsset7dAnd30dValues(ConcurrentDictionary<int, Tuple<double?, double?>> values)
        {
            var currentValues = new ConcurrentBag<AssetCurrentValue>();
            Parallel.ForEach(values, new ParallelOptions() { MaxDegreeOfParallelism = 7 }, assetToUpdate =>
            {
                currentValues.Add(new AssetCurrentValue()
                {
                    Id = assetToUpdate.Key,
                    Variation7Days = assetToUpdate.Value.Item1,
                    Variation30Days = assetToUpdate.Value.Item2
                });
            });
            AssetCurrentValueBusiness.UpdateAssetValue7And30Days(currentValues);
        }
    }
}
