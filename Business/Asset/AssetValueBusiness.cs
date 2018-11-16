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
            Dictionary<int, Tuple<double, double>> currentValues = GetAssetsCurrentValuesAndVariationFromBinanceTicker();

            var advisors = AdvisorBusiness.GetAdvisors();
            var advices = AdviceBusiness.List(advisors.Select(c => c.Id));
            var currentDate = Data.GetDateTimeNow();
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);

            Parallel.Invoke(() => UpdateAssetCurrentValues(currentDate, currentValues),
                            () => ClosePositionForStopLossAndTargetPriceReached(currentDate, advices, currentValues.ToDictionary(c => c.Key, c => c.Value.Item1)));
        }

        private Dictionary<int, Tuple<double, double>> GetAssetsCurrentValuesAndVariationFromBinanceTicker()
        {
            var ticker = BinanceBusiness.GetTicker24h();
            var pairs = PairBusiness.ListPairs();
            var usdtPairs = pairs.Where(p => p.QuoteAssetId == AssetUSDId);
            var btcPairs = pairs.Where(p => !usdtPairs.Any(usdtPair => usdtPair.BaseAssetId == p.BaseAssetId) && p.QuoteAssetId == AssetBTCId);

            var currentValues = new Dictionary<int, Tuple<double, double>>();

            foreach (var usdtPair in usdtPairs)
            {
                var currentTicker = ticker.FirstOrDefault(t => t.Symbol == usdtPair.Symbol);
                if (currentTicker != null)
                    currentValues.Add(usdtPair.BaseAssetId, new Tuple<double, double>(currentTicker.LastPrice, currentTicker.PriceChangePercent / 100.0));
            }

            foreach (var btcPair in btcPairs)
            {
                var currentTicker = ticker.FirstOrDefault(t => t.Symbol == btcPair.Symbol);
                if (currentTicker != null && currentValues.ContainsKey(btcPair.QuoteAssetId))
                {
                    var variation24h = (1 * (currentTicker.PriceChangePercent / 100.0 + 1) * (1 + currentValues[btcPair.QuoteAssetId].Item2)) - 1;
                    currentValues.Add(btcPair.BaseAssetId, new Tuple<double, double>(currentTicker.LastPrice * currentValues[btcPair.QuoteAssetId].Item1, variation24h));
                }
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
            var usdtPairs = pairs.Where(p => p.QuoteAssetId == AssetUSDId);
            var btcPairs = pairs.Where(p => !usdtPairs.Any(usdtPair => usdtPair.BaseAssetId == p.BaseAssetId) && p.QuoteAssetId == AssetBTCId);

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
                    var totalVariation7d = GetVariationWithMultiplierQuote(variation.Item1 , currentValues[btcPair.QuoteAssetId].Item1);
                    var totalVariation30d = GetVariationWithMultiplierQuote(variation.Item2, currentValues[btcPair.QuoteAssetId].Item2);
                    currentValues.TryAdd(btcPair.BaseAssetId, new Tuple<double?, double?>(totalVariation7d, totalVariation30d));
                });

            return currentValues;
        }

        private static double? GetVariationWithMultiplierQuote(double? variationBase, double? variationQuote)
        {
            return (1 + variationBase) * (1 + variationQuote) - 1;
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
            return (kline.Close - kline.Open) / kline.Open;
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
                    var assetLastValue = lastValues[advice.AssetId];
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

        public void UpdateCoingeckoAssetsValues()
        {
            var currentDate = Data.GetDateTimeNow();
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);

            List<DomainObjects.Asset.Asset> assets = null;
            List<Advice> advices = null;
            List<AssetCurrentValue> assetCurrentValues = null;
            IEnumerable<AssetResult> assetResults = null;
            List<Pair> pairs = null;
            var advisorsId = AdvisorBusiness.GetAdvisors().Select(c => c.Id).Distinct().ToHashSet();
            Parallel.Invoke(
                () => assets = AssetBusiness.ListAssets(),
                () => assetResults = CoinGeckoBusiness.GetAllCoinsData(),
                () => pairs = PairBusiness.ListPairs(),
                () => advices = AdviceBusiness.List(advisorsId), 
                () => assetCurrentValues = AssetCurrentValueBusiness.ListAllAssets());

            var consideredAssetsId = assets.Where(c => advices.Any(a => a.AssetId == c.Id) && !pairs.Any(p => p.BaseAssetId == c.Id)).Select(c => c.Id).Distinct().ToHashSet();

            var currentValues = new Dictionary<int, AssetResult>();
            foreach (var assetValue in assetResults.Where(c => c.Price.HasValue))
            {
                var asset = assets.FirstOrDefault(c => c.CoinGeckoId == assetValue.Id);
                if (asset != null && !pairs.Any(p => p.BaseAssetId == asset.Id))
                    currentValues[asset.Id] = assetValue;
            }

            var assetsToUpdateLastValues = assetCurrentValues.Where(c => currentValues.ContainsKey(c.Id)).ToList();
            if (assetsToUpdateLastValues.Any())
            {
                Parallel.Invoke(() => UpdateAssetCurrentValues(currentDate, currentValues, consideredAssetsId, assetsToUpdateLastValues),
                                () => ClosePositionForStopLossAndTargetPriceReached(currentDate, advices, currentValues.ToDictionary(c => c.Key, c => c.Value.Price.Value)));
            }
        }

        private void UpdateAssetCurrentValues(DateTime currentDate, Dictionary<int, AssetResult> lastValues, HashSet<int> consideredAssetsId,
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
                            CurrentValue = lastAssetValue.Price.Value,
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
                            CurrentValue = lastAssetValue.Price.Value,
                            Variation24Hours = lastAssetValue.VariationPercentage24h.HasValue ? lastAssetValue.VariationPercentage24h.Value / 100.0 : (double?)null
                        });
                    }
                }
                else
                {
                    currentValues.Add(new AssetCurrentValue()
                    {
                        Id = assetToUpdate.Id,
                        UpdateDate = currentDate,
                        CurrentValue = lastAssetValue.Price.Value,
                        Variation24Hours = lastAssetValue.VariationPercentage24h.HasValue ? lastAssetValue.VariationPercentage24h.Value / 100.0 : (double?)null
                    });
                }
            });
            AssetCurrentValueBusiness.UpdateFullAssetCurrentValues(currentValues);
        }
    }
}
