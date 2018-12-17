using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.DomainObjects.Trade;
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

        public List<AssetResponse.ValuesResponse> FilterValueResponse(int assetId, DateTime? dateTime)
        {
            dateTime = dateTime ?? Data.GetDateTimeNow().AddDays(-30);
            var asset = AssetBusiness.GetById(assetId);
            var days = Math.Ceiling(Data.GetDateTimeNow().Subtract(dateTime.Value).TotalDays);

            return CoinGeckoBusiness.GetAssetValues(asset.CoinGeckoId, (int)days);
        }

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

        public Dictionary<int, Dictionary<OrderActionType, List<OrderResponse>>> UpdateBinanceAssetsValues()
        {
            Dictionary<int, TickerDataModel> currentValues = GetAssetsCurrentValuesAndVariationFromBinanceTicker();
            
            var currentDate = Data.GetDateTimeNow();
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);

            Dictionary<int, Dictionary<OrderActionType, List<OrderResponse>>> result = null;
            Parallel.Invoke(() => UpdateAssetCurrentValues(currentDate, currentValues),
                            () => result = OrderBusiness.ClosePositionForStopLossAndTargetPriceReached(currentDate, currentValues));

            return result;
        }

        private Dictionary<int, TickerDataModel> GetAssetsCurrentValuesAndVariationFromBinanceTicker()
        {
            var ticker = BinanceBusiness.GetTicker24h();
            var pairs = PairBusiness.ListPairs();
            var usdtPairs = pairs.Where(p => p.QuoteAssetId == AssetUSDId);
            var btcPairs = pairs.Where(p => !usdtPairs.Any(usdtPair => usdtPair.BaseAssetId == p.BaseAssetId) && p.QuoteAssetId == AssetBTCId);

            var currentValues = new Dictionary<int, TickerDataModel>();

            foreach (var usdtPair in usdtPairs)
            {
                var currentTicker = ticker.FirstOrDefault(t => t.Symbol == usdtPair.Symbol);
                if (currentTicker != null)
                    currentValues.Add(usdtPair.BaseAssetId, new TickerDataModel()
                    {
                        CurrentValue = currentTicker.LastPrice,
                        Variation24Hours = currentTicker.PriceChangePercent / 100.0,
                        AskValue = currentTicker.AskPrice,
                        BidValue = currentTicker.BidPrice
                    });
            }

            foreach (var btcPair in btcPairs)
            {
                var currentTicker = ticker.FirstOrDefault(t => t.Symbol == btcPair.Symbol);
                if (currentTicker != null && currentValues.ContainsKey(btcPair.QuoteAssetId))
                {
                    currentValues.Add(btcPair.BaseAssetId, new TickerDataModel()
                    {
                        CurrentValue = currentTicker.LastPrice * currentValues[btcPair.QuoteAssetId].CurrentValue,
                        Variation24Hours = GetVariation24h(currentTicker.LastPrice, currentTicker.PriceChangePercent / 100.0, currentValues[btcPair.QuoteAssetId].CurrentValue, currentValues[btcPair.QuoteAssetId].Variation24Hours.Value),
                        AskValue = currentTicker.AskPrice * currentValues[btcPair.QuoteAssetId].AskValue,
                        BidValue = currentTicker.BidPrice * currentValues[btcPair.QuoteAssetId].BidValue
                    });
                }
            }

            return currentValues;
        }

        public double GetVariation24h(double assetCurrentValue, double assetVariantion24h, double btcCurrentValue, double btcVariation24h)
        {
            return (assetCurrentValue * btcCurrentValue) / ((assetCurrentValue * (1 + (-1 * assetVariantion24h))) * (btcCurrentValue * (1 + (-1 * btcVariation24h)))) - 1;
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

        private void UpdateAssetCurrentValues(DateTime currentDate, Dictionary<int, TickerDataModel> lastValues)
        {
            var currentValues = new ConcurrentBag<AssetCurrentValue>();
            Parallel.ForEach(lastValues, new ParallelOptions() { MaxDegreeOfParallelism = 7 }, assetToUpdate =>
            {
                currentValues.Add(new AssetCurrentValue()
                {
                    Id = assetToUpdate.Key,
                    UpdateDate = currentDate,
                    CurrentValue = assetToUpdate.Value.CurrentValue,
                    Variation24Hours = assetToUpdate.Value.Variation24Hours,
                    AskValue = assetToUpdate.Value.AskValue,
                    BidValue = assetToUpdate.Value.BidValue
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
