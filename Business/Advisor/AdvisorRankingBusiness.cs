using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.DomainObjects.Trade;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Auctus.Business.Advisor.AdvisorRankingBusiness.AdvisorRankingAndProfitData;
using static Auctus.Model.AdvisorResponse;

namespace Auctus.Business.Advisor
{
    public class AdvisorRankingBusiness : BaseBusiness<AdvisorRanking, IAdvisorRankingData<AdvisorRanking>>
    {
        private const string ADVISORS_RANKING_CACHE_KEY = "AdvisorsRanking";
        public AdvisorRankingBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<AdvisorRanking> ListAdvisorsFullData()
        {
            var advisors = MemoryCache.Get<List<AdvisorRanking>>(ADVISORS_RANKING_CACHE_KEY);
            if (advisors == null)
            {
                advisors = Data.ListAdvisorsRankingAndProfit(null, null);
                UpdateAdvisorsFullDataCache(advisors.ToList());
            }
            return advisors;
        }

        public void AppendNewAdvisorToCache(int advisorId)
        {
            var user = AdvisorBusiness.GetAdvisor(advisorId);
            var advisors = ListAdvisorsFullData();
            advisors.Add(GetStartAdvisorRanking(user, user.BecameAdvisorDate, advisors.Count));
        }

        public AdvisorRanking GetAdvisorSimpleData(int advisorId)
        {
            var advisors = MemoryCache.Get<List<AdvisorRanking>>(ADVISORS_RANKING_CACHE_KEY);
            if (advisors == null || !advisors.Any(c => c.Id == advisorId))
                return Data.ListAdvisorsRanking(new int[] { advisorId }).FirstOrDefault();
            else
                return advisors.FirstOrDefault(c => c.Id == advisorId);
        }

        public AdvisorRanking GetAdvisorFullData(int advisorId)
        {
            var advisors = ListAdvisorsFullData();
            var advisor = advisors.FirstOrDefault(c => c.Id == advisorId);
            if (advisor != null)
            {
                advisor.IsAdvisor = true;
                advisor.TotalAdvisors = advisors.Count;
                return advisor;
            }
            else
            {
                var advisorData = Data.ListAdvisorsRankingAndProfit(new int[] { advisorId }, null).FirstOrDefault();
                if (advisorData != null)
                {
                    advisors.Add(advisorData);
                    advisorData.IsAdvisor = true;
                    advisorData.TotalAdvisors = advisors.Count;
                    return advisorData;
                }
                else
                {
                    var user = AdvisorBusiness.GetAdvisor(advisorId);
                    var now = Data.GetDateTimeNow();
                    return GetStartAdvisorRanking(user, now, advisors.Count);
                }
            }
        }

        private AdvisorRanking GetStartAdvisorRanking(DomainObjects.Advisor.Advisor advisor, DateTime referenceDate, int totalAdvisors)
        {
            return advisor == null || !advisor.Enabled ? null : new AdvisorRanking()
            {
                Id = advisor.Id,
                Email = advisor.Email,
                Password = advisor.Password,
                CreationDate = advisor.CreationDate,
                ConfirmationCode = advisor.ConfirmationCode,
                ConfirmationDate = advisor.ConfirmationDate,
                ReferralCode = advisor.ReferralCode,
                ReferralStatus = advisor.ReferralStatus,
                BonusToReferred = advisor.BonusToReferred,
                ReferredId = advisor.ReferredId,
                AllowNotifications = advisor.AllowNotifications,
                DiscountProvided = advisor.DiscountProvided,
                IsAdvisor = true,
                Name = advisor.Name,
                Description = advisor.Description,
                UrlGuid = advisor.UrlGuid,
                BecameAdvisorDate = advisor.BecameAdvisorDate,
                Enabled = advisor.Enabled,
                UpdateDate = referenceDate,
                Rating = 2.5,
                Ranking = totalAdvisors + 1,
                TotalAdvisors = totalAdvisors + 1,
                AdvisorProfit = new List<AdvisorProfit>() { AdvisorProfitBusiness.GetBaseUsdAdvisorProfit(advisor.Id, referenceDate) }
            };
        }

        public List<AdvisorProfit> GetAdvisorRankingAndProfitData(int advisorId, int assetId, DateTime now, List<Order> advisorAssetOrders, double? assetCurrentValue)
        {
            var assetsCurrentValue = new Dictionary<int, double>();
            if (assetCurrentValue.HasValue)
                assetsCurrentValue[assetId] = assetCurrentValue.Value;
            return BuildAdvisorsProfit(advisorId, now, GetAdvisorRankingAndProfitData(now, advisorAssetOrders, assetsCurrentValue).AssetProfitData);
        }

        public AdvisorResponse GetAdvisorResponse(AdvisorRanking advisorRanking, int totalAdvisors, IEnumerable<FollowAdvisor> advisorFollowers, User loggedUser, 
            List<DomainObjects.Asset.Asset> assets, AdvisorRankingHistory advisorLastDayHistory, AdvisorRankingHistory advisorMonthBeginningHistory, 
            AdvisorMonthlyRanking advisorMonthlyRanking)
        {
            var advFollowers = advisorFollowers?.Where(c => c.AdvisorId == advisorRanking.Id);
            var totalAvailable = advisorRanking.AdvisorProfit.First(c => c.AssetId == AssetUSDId).TotalDollar;
            var totalOpenOrders = advisorRanking.AdvisorProfit.Where(c => c.AssetId != AssetUSDId && c.OrderStatusType == OrderStatusType.Open).Sum(c => c.TotalDollar);
            var totalAllocated = advisorRanking.AdvisorProfit.Where(c => c.AssetId != AssetUSDId && c.OrderStatusType == OrderStatusType.Executed).Sum(c => c.TotalDollar - c.SummedProfitDollar);
            var consideredProfits = advisorRanking.AdvisorProfit.Where(c => c.AssetId != AssetUSDId && c.OrderStatusType != OrderStatusType.Open);
            var historyLastDayTotalAvailable = advisorLastDayHistory?.AdvisorProfitHistory.First(c => c.AssetId == AssetUSDId).TotalDollar;
            var historyLastDayTotalAllocated = advisorLastDayHistory?.AdvisorProfitHistory.Where(c => c.AssetId != AssetUSDId && c.OrderStatusType != OrderStatusType.Close).Sum(c => c.TotalDollar);
            var historyLastDayConsideredProfits = advisorLastDayHistory?.AdvisorProfitHistory.Where(c => c.AssetId != AssetUSDId && c.OrderStatusType != OrderStatusType.Open);
            var runningProfits = consideredProfits.Where(c => c.OrderStatusType == OrderStatusType.Executed);
            var assetsTraded = consideredProfits.Select(c => c.AssetId);
            var totalOrders = consideredProfits.Any() ? consideredProfits.Sum(c => c.OrderCount) : 0;
            var successRate = totalOrders > 0 ? (double)consideredProfits.Sum(c => c.SuccessCount) / totalOrders : 0.0;
            var averageReturn = totalOrders > 0 ? consideredProfits.Sum(c => c.SummedProfitDollar) / consideredProfits.Sum(c => c.TotalDollar - c.SummedProfitDollar) : 0.0;
            var openProfit = runningProfits.Any() ? runningProfits.Sum(c => c.SummedProfitDollar) : 0.0;
            var totalVirtual = totalAvailable + totalAllocated + totalOpenOrders + openProfit;
            var historyLastDayPortfolioValue = historyLastDayTotalAvailable.HasValue ? historyLastDayTotalAvailable + historyLastDayTotalAllocated : (double?)null;
            var allProfit = totalVirtual - VirtualMoney;
            var allProfitPercentage = totalVirtual / VirtualMoney - 1;
            var closedOrdersWithTime = consideredProfits.Where(c => c.OrderStatusType == OrderStatusType.Close && c.SummedTradeMinutes.HasValue);
            var averageTradeMinutes = closedOrdersWithTime.Any() ? closedOrdersWithTime.Sum(c => c.SummedTradeMinutes.Value) / closedOrdersWithTime.Sum(c => c.OrderCount) : (int?)null;
    
            List<AdvisorAssetResponse> openPositions = null;
            List<AdvisorAssetResponse> closedPositions = null;
            closedPositions = consideredProfits.Where(c => c.OrderStatusType == OrderStatusType.Close).Select(c => GetAdvisorAssetResponse(c, assets)).Where(c => c != null).OrderByDescending(c => c.OrderCount).ToList();
            openPositions = runningProfits.Select(c => GetAdvisorAssetResponse(c, assets)).Where(c => c != null).OrderByDescending(c => c.TotalInvested).ToList();
            
            List<AdvisorAssetHistoryResponse> assetsHistory = new List<AdvisorAssetHistoryResponse>();
            if (historyLastDayConsideredProfits?.Any() == true)
            {
                var assetData = historyLastDayConsideredProfits.GroupBy(c => c.AssetId);
                foreach(var data in assetData)
                {
                    var assetCurrentValue = 0.0;
                    var assetCurrentData = closedPositions.Where(c => c.AssetId == data.Key).Concat(openPositions.Where(c => c.AssetId == data.Key));
                    if (assetCurrentData.Any())
                        assetCurrentValue = assetCurrentData.Sum(c => c.TotalVirtual);

                    var totalDollar = data.Sum(c => c.TotalDollar);
                    var totalQuantity = data.Sum(c => c.TotalQuantity);
                    var totalProfit = data.Sum(c => c.SummedProfitDollar);
                    var assetLastValue = totalDollar + totalProfit;
                    assetsHistory.Add(new AdvisorAssetHistoryResponse()
                    {
                        AssetId = data.Key,
                        TotalInvested = totalDollar,
                        TotalQuantity = totalQuantity,
                        TotalProfit = totalProfit,
                        TotalVirtual = assetLastValue,
                        AveragePrice = totalDollar / totalQuantity,
                        Profit24hValue = assetCurrentValue != 0 ? assetCurrentValue - assetLastValue : (double?)null,
                        Profit24hPercentage = assetCurrentValue != 0 ? assetCurrentValue / assetLastValue - 1 : (double?)null
                    });
                }
            }
            return new AdvisorResponse()
            {
                UserId = advisorRanking.Id,
                Name = advisorRanking.Name,
                UrlGuid = advisorRanking.UrlGuid.ToString(),
                CreationDate = advisorRanking.BecameAdvisorDate,
                Description = advisorRanking.Description,
                Owner = loggedUser != null && advisorRanking.Id == loggedUser.Id,
                NumberOfFollowers = advFollowers?.Count() ?? 0,
                TotalAssetsTraded = assetsTraded.Any() ? assetsTraded.Distinct().Count() : 0,
                TotalTrades = totalOrders,
                Following = loggedUser != null && loggedUser?.FollowedAdvisors?.Any(c => c == advisorRanking.Id) == true,
                AverageReturn = averageReturn,
                SuccessRate = successRate,
                Ranking = advisorRanking.Ranking,
                Rating = advisorRanking.Rating,
                TotalAdvisors = totalAdvisors,
                TotalAllocated = totalAllocated,
                TotalAvailable = totalAvailable,
                TotalBalance = totalOpenOrders + totalAvailable,
                Equity = totalVirtual,
                TotalProfit = allProfit,
                TotalProfitPercentage = allProfitPercentage,
                AverageTradeMinutes = averageTradeMinutes,
                ClosedPositions = closedPositions,
                OpenPositions = openPositions,
                LastPortfolioReferenceDate = advisorLastDayHistory?.ReferenceDate,
                LastPortfolioValue = historyLastDayPortfolioValue,
                Profit24hValue = historyLastDayPortfolioValue.HasValue ? totalVirtual - historyLastDayPortfolioValue : (double?)null,
                Profit24hPercentage = historyLastDayPortfolioValue.HasValue ? totalVirtual / historyLastDayPortfolioValue - 1 : (double?)null,
                AdvisorAsset24hHistory = assetsHistory,
                MonthlyRankingHistory = GetMonthlyRankingHistoryResponse(totalVirtual, advisorMonthBeginningHistory, advisorMonthlyRanking),
                RecommendationDistribution = runningProfits.Any() ? runningProfits.GroupBy(c => c.Type).Select(g => new RecommendationDistributionResponse() { Type = g.Key, Total = g.Sum(c => c.OrderCount) }).ToList() : new List<RecommendationDistributionResponse>(),
            };
        }

        private MonthlyRankingHistoryResponse GetMonthlyRankingHistoryResponse(double equity, AdvisorRankingHistory advisorMonthBeginningHistory, AdvisorMonthlyRanking advisorMonthlyRanking)
        {
            if (advisorMonthBeginningHistory != null)
            {
                var historyMonthBeginningPortfolioValue = advisorMonthBeginningHistory.AdvisorProfitHistory.Where(c => c.OrderStatusType != OrderStatusType.Close).Sum(c => c.TotalDollar);
                return new MonthlyRankingHistoryResponse()
                {
                    PortfolioReferenceDate = advisorMonthBeginningHistory.ReferenceDate,
                    PortfolioValue = historyMonthBeginningPortfolioValue,
                    ProfitPercentage = equity / historyMonthBeginningPortfolioValue - 1
                };
            }
            else if (advisorMonthlyRanking != null)
            {
                return new MonthlyRankingHistoryResponse()
                {
                    Ranking = advisorMonthlyRanking.Ranking,
                    ProfitPercentage = advisorMonthlyRanking.AverageReturn
                };
            }
            else
                return null;
        }

        private AdvisorAssetResponse GetAdvisorAssetResponse(AdvisorProfit advisorProfit, List<DomainObjects.Asset.Asset> assets)
        {
            var asset = assets?.FirstOrDefault(c => c.Id == advisorProfit.AssetId);
            return new AdvisorAssetResponse()
                {
                    AssetId = advisorProfit.AssetId,
                    AssetName = asset?.Name,
                    AssetCode = asset?.Code,
                    Pair = PairBusiness.GetBaseQuotePair(advisorProfit.AssetId),
                    Type = advisorProfit.Type,
                    OrderCount = advisorProfit.OrderCount,
                    SuccessCount = advisorProfit.SuccessCount,
                    TotalInvested = advisorProfit.TotalDollar - advisorProfit.SummedProfitDollar,
                    TotalQuantity = advisorProfit.TotalQuantity,
                    TotalProfit = advisorProfit.SummedProfitDollar,
                    AveragePrice = (advisorProfit.TotalDollar - advisorProfit.SummedProfitDollar) / advisorProfit.TotalQuantity,
                    SuccessRate = (double)advisorProfit.SuccessCount / advisorProfit.OrderCount,
                    AverageReturn = advisorProfit.SummedProfitDollar / (advisorProfit.TotalDollar - advisorProfit.SummedProfitDollar),
                    TotalVirtual = advisorProfit.TotalDollar,
                    SummedTradeMinutes = advisorProfit.SummedTradeMinutes
                };
        }

        private void UpdateAdvisorsFullDataCache(List<AdvisorRanking> advisors)
        {
            if (advisors != null && advisors.Any())
                MemoryCache.Set(ADVISORS_RANKING_CACHE_KEY, advisors, 10);
        }

        public void SetAdvisorRankingAndProfit()
        {
            var advisors = AdvisorBusiness.ListAllAdvisors().Select(c => c.Id).Distinct().ToList();
            List<Order> orders = null;
            Dictionary<int, double> assetsCurrentValues = null;
            List<Order> finishedOrders = null;
            Parallel.Invoke(() => orders = OrderBusiness.ListOrdersForRankingProfitCalculation(advisors),
                            () => assetsCurrentValues = AssetCurrentValueBusiness.ListAllAssets(true).ToDictionary(c => c.Id, c => c.CurrentValue),
                            () => finishedOrders = OrderBusiness.ListOrders(advisors, null, new OrderStatusType[] { OrderStatusType.Finished }));

            var now = Data.GetDateTimeNow();
            var groupedOrders = orders.GroupBy(c => c.UserId).ToDictionary(c => c.Key, c => c.ToList());
            var advisorRankingAndProfitData = new Dictionary<int, AdvisorRankingAndProfitData>();
            foreach (var advisorOrders in groupedOrders)
                advisorRankingAndProfitData[advisorOrders.Key] = GetAdvisorRankingAndProfitData(now, advisorOrders.Value, assetsCurrentValues);

            var totalCount = advisorRankingAndProfitData.Sum(c => c.Value.OrderCount);
            if (totalCount == 0)
                return;

            var totalWeight = advisorRankingAndProfitData.Sum(c => c.Value.RankingWeight);
            var generalAvg = totalWeight != 0 ? advisorRankingAndProfitData.Sum(c => c.Value.RankingWeightedProfit) / totalWeight : 0;
            var weightedStdDivisor = totalWeight * (totalCount - 1) / totalCount;
            var weightedStd = Math.Sqrt(advisorRankingAndProfitData.Where(c => c.Value.RankingWeight != 0 && c.Value.OrderCount > 0)
                .Sum(c => (Math.Pow((c.Value.RankingWeightedProfit / c.Value.RankingWeight) - generalAvg, 2) * c.Value.RankingWeight) / weightedStdDivisor));

            var consideredAdvisors = advisorRankingAndProfitData.Where(c => c.Value.RankingWeight != 0 && c.Value.OrderCount > 0);
            var z = new Dictionary<int, double>();
            var minZ = 0.0;
            var normalizationDivisor = 1.0;
            if (weightedStd != 0)
            {
                z = consideredAdvisors.ToDictionary(c => c.Key, c => ((c.Value.RankingWeightedProfit / c.Value.RankingWeight) - generalAvg) / (weightedStd / Math.Sqrt(c.Value.OrderCount)));
                minZ = z.Min(c => c.Value);
                normalizationDivisor = z.Max(c => c.Value) - minZ;
            }
            else
            {
                var value = 1.0001;
                z = consideredAdvisors.OrderByDescending(c => c.Value.OrderCount).ToDictionary(c => c.Key, c =>
                    {
                        value -= 0.0001;
                        return value;
                    });
            }

            var advisorRating = new Dictionary<int, double>();
            advisors.ForEach(c =>
            {
                if (!z.ContainsKey(c))
                    advisorRating[c] = 2.5;
                else
                    advisorRating[c] = 2.501 + (2.499 * ((z[c] - minZ) / normalizationDivisor));
            });
            var advisorsOrdered = advisorRating.OrderByDescending(c => c.Value).ThenByDescending(c => c.Key);

            var advisorsRanking = new List<AdvisorRanking>();
            var advisorsProfit = new List<AdvisorProfit>();
            for (int i = 0; i < advisorsOrdered.Count(); ++i)
            {
                var id = advisorsOrdered.ElementAt(i).Key;
                advisorsRanking.Add(new AdvisorRanking()
                {
                    Id = id,
                    UpdateDate = now,
                    Ranking = i + 1,
                    Rating = advisorsOrdered.ElementAt(i).Value
                });

                var usdPosition = AdvisorProfitBusiness.GetBaseUsdAdvisorProfit(id, now);
                if (advisorRankingAndProfitData.ContainsKey(id))
                {
                    var advisorProfit = BuildAdvisorsProfit(id, now, advisorRankingAndProfitData[id].AssetProfitData);
                    if (advisorProfit.Any())
                    {
                        var closedPositions = groupedOrders[id].Where(c => c.OrderStatusType == OrderStatusType.Close);
                        if (closedPositions.Any())
                            usdPosition.TotalDollar += closedPositions.Sum(c => GetCloseOrderTotalDollar(c));
                        
                        var openOrdersPositions = groupedOrders[id].Where(c => c.AssetId != AssetUSDId && (c.OrderStatusType == OrderStatusType.Open || c.OrderStatusType == OrderStatusType.Executed));
                        if (openOrdersPositions.Any())
                            usdPosition.TotalDollar -= openOrdersPositions.Sum(c => c.Price * c.Quantity);

                        var userFinishedOrders = finishedOrders.Where(c => c.UserId == id);
                        if (userFinishedOrders.Any())
                            usdPosition.TotalDollar -= userFinishedOrders.Sum(c => c.Price * c.Quantity);

                        usdPosition.TotalQuantity = usdPosition.TotalDollar;
                        advisorsProfit.AddRange(advisorProfit);
                    }
                }
                advisorsProfit.Add(usdPosition);
            }

            Parallel.Invoke(() => Data.SetAdvisorRanking(advisorsRanking),
                            () => AdvisorProfitBusiness.SetAdvisorProfit(advisorsProfit));

            UpdateAdvisorsFullDataCache(Data.ListAdvisorsRankingAndProfit(null, null));
        }

        private double GetCloseOrderTotalDollar(Order order)
        {
            if (order.OrderType.GetOppositeType() == OrderType.Buy)
                return order.Quantity * order.Price;
            else
                return order.Quantity * (1.0 + order.Profit.Value) * (order.Price / (1 - order.Profit.Value));
        }

        private List<AdvisorProfit> BuildAdvisorsProfit(int advisorId, DateTime now, Dictionary<int, Dictionary<OrderStatusType, Dictionary<OrderType, ProfitData>>> advisorAssetProfitData)
        {
            var advisorsProfit = new List<AdvisorProfit>();
            foreach (var advisorAssetProfit in advisorAssetProfitData)
            {
                foreach (var assetData in advisorAssetProfit.Value)
                {
                    foreach (var data in assetData.Value)
                    {
                        advisorsProfit.Add(new AdvisorProfit()
                        {
                            UserId = advisorId,
                            UpdateDate = now,
                            AssetId = advisorAssetProfit.Key,
                            Status = assetData.Key.Value,
                            Type = data.Key.Value,
                            OrderCount = data.Value.OrderCount,
                            SummedProfitPercentage = data.Value.SummedProfitPercentage,
                            TotalDollar = data.Value.TotalDollar,
                            TotalQuantity = data.Value.TotalQuantity,
                            SummedProfitDollar = data.Value.SummedProfitDollar,
                            SuccessCount = data.Value.SuccessCount,
                            SummedTradeMinutes = data.Value.SummedTradeMinutes
                        });
                    }
                }
            }
            return advisorsProfit;
        }

        public class AdvisorRankingAndProfitData
        {
            public int OrderCount { get; set; } = 0;
            public double RankingWeight { get; set; } = 0;
            public double RankingWeightedProfit { get; set; } = 0;
            public Dictionary<int, Dictionary<OrderStatusType, Dictionary<OrderType, ProfitData>>> AssetProfitData { get; set; } = new Dictionary<int, Dictionary<OrderStatusType, Dictionary<OrderType, ProfitData>>>();

            public class ProfitData
            {
                public double SummedProfitPercentage { get; set; } = 0;
                public double SummedProfitDollar { get; set; } = 0;
                public double TotalDollar { get; set; } = 0;
                public double TotalQuantity { get; set; } = 0;
                public int SuccessCount { get; set; } = 0;
                public int OrderCount { get; set; } = 0;
                public int? SummedTradeMinutes { get; set; } = null;
            }
        }

        private AdvisorRankingAndProfitData GetAdvisorRankingAndProfitData(DateTime now, List<Order> advisorClosedOpenAndRunningOrders, Dictionary<int, double> assetsCurrentValues)
        {
            var result = new AdvisorRankingAndProfitData();
            var advisorClosedOrders = advisorClosedOpenAndRunningOrders.Where(c => c.OrderStatusType == OrderStatusType.Close);
            foreach (var order in advisorClosedOrders)
            {
                var tradeMinutes = Convert.ToInt32(Math.Round(order.StatusDate.Subtract(order.OpenDate.Value).TotalMinutes, 0));
                SetAdvisorRankingAndProfitData(ref result, order.OrderStatusType, order.OrderType, order.AssetId, order.Profit.Value, order.Quantity, GetCloseOrderTotalDollar(order), now, order.StatusDate, tradeMinutes);
            }

            var advisorOpenOrders = advisorClosedOpenAndRunningOrders.Where(c => c.OrderStatusType == OrderStatusType.Open);
            foreach (var order in advisorOpenOrders)
                SetAdvisorRankingAndProfitData(ref result, order.OrderStatusType, order.OrderType, order.AssetId, 0, order.Quantity, order.Price * order.Quantity, now, order.StatusDate, null);

            var advisorRunningOrders = advisorClosedOpenAndRunningOrders.Where(c => c.OrderStatusType == OrderStatusType.Executed && c.AssetId != AssetUSDId);
            if (advisorRunningOrders.Any())
            {
                var assetsAvgPrice = advisorRunningOrders.GroupBy(c => new { c.AssetId, c.Type }).Select(c =>  new
                {
                    AssetId = c.Key.AssetId,
                    OrderType = OrderType.Get(c.Key.Type),
                    TotalUSD = c.Sum(s => s.RemainingQuantity * s.Price),
                    TotalQuantity = c.Sum(s => s.RemainingQuantity)
                });
                foreach (var avgPrice in assetsAvgPrice)
                {
                    var price = (avgPrice.TotalUSD / avgPrice.TotalQuantity);
                    var currentValue = assetsCurrentValues.ContainsKey(avgPrice.AssetId) ? assetsCurrentValues[avgPrice.AssetId] : price;
                    var profit = (avgPrice.OrderType == OrderType.Buy ? 1.0 : -1.0) * (currentValue / price - 1);
                    var totalDollar = avgPrice.TotalUSD + (profit * avgPrice.TotalUSD);
                    SetAdvisorRankingAndProfitData(ref result, OrderStatusType.Executed, avgPrice.OrderType, avgPrice.AssetId, profit, avgPrice.TotalQuantity, totalDollar, now, now, null);
                }
            }
            return result;
        }

        private void SetAdvisorRankingAndProfitData(ref AdvisorRankingAndProfitData advisorRankingAndProfitData, OrderStatusType statusType, OrderType orderType, 
            int assetId, double profit, double quantity, double totalUSD, DateTime now, DateTime closeDate, int? tradeMinutes)
        {
            if (statusType != OrderStatusType.Open)
            {
                var days = now.Subtract(closeDate).TotalDays;
                advisorRankingAndProfitData.RankingWeight += (days <= 30 ? 1.0 : Math.Max((Math.Log(days) / -2.5100067169575757) + 2.3550550915595987, 0.0));
                advisorRankingAndProfitData.RankingWeightedProfit += profit * advisorRankingAndProfitData.RankingWeight;
                if (advisorRankingAndProfitData.RankingWeight > 0)
                    ++advisorRankingAndProfitData.OrderCount;
            }
            
            if (!advisorRankingAndProfitData.AssetProfitData.ContainsKey(assetId))
                advisorRankingAndProfitData.AssetProfitData[assetId] = new Dictionary<OrderStatusType, Dictionary<OrderType, AdvisorRankingAndProfitData.ProfitData>>();
            if (!advisorRankingAndProfitData.AssetProfitData[assetId].ContainsKey(statusType))
                advisorRankingAndProfitData.AssetProfitData[assetId][statusType] = new Dictionary<OrderType, AdvisorRankingAndProfitData.ProfitData>();
            if (!advisorRankingAndProfitData.AssetProfitData[assetId][statusType].ContainsKey(orderType))
                advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType] = new AdvisorRankingAndProfitData.ProfitData();

            advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].SummedProfitPercentage += profit;
            advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].TotalDollar += totalUSD;
            advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].TotalQuantity += quantity;
            advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].SummedProfitDollar += totalUSD - (totalUSD / (1 + profit));
            ++advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].OrderCount;
            if (profit > 0)
                ++advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].SuccessCount;

            if (tradeMinutes.HasValue)
            {
                if (advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].SummedTradeMinutes.HasValue)
                    advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].SummedTradeMinutes += tradeMinutes.Value;
                else
                    advisorRankingAndProfitData.AssetProfitData[assetId][statusType][orderType].SummedTradeMinutes = tradeMinutes.Value;
            }
        }
    }
}
