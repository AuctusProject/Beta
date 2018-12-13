using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Trade;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Auctus.Model.AdvisorPerformanceResponse;

namespace Auctus.Business.Advisor
{
    public class AdvisorRankingHistoryBusiness : BaseBusiness<AdvisorRankingHistory, IAdvisorRankingHistoryData<AdvisorRankingHistory>>
    {
        public AdvisorRankingHistoryBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public void SetAdvisorRankingAndProfitHistory()
        {
            var data = AdvisorRankingBusiness.ListAdvisorsFullData();
            var now = Data.GetDateTimeNow();

            Parallel.Invoke(() => Data.SetAdvisorRankingHistory(now, data),
                            () => AdvisorProfitHistoryBusiness.SetAdvisorProfitHistory(now, data.SelectMany(c => c.AdvisorProfit)));
        }

        public AdvisorRankingHistory GetLastAdvisorRankingAndProfit(int advisorId)
        {
            var cacheKey = $"AdvisorsHistory_{advisorId}";
            var historyData = MemoryCache.Get<AdvisorRankingHistory>(cacheKey);
            if (historyData == null)
            {
                var now = Data.GetDateTimeNow();
                historyData = Data.ListAdvisorRankingAndProfitHistory(new int[] { advisorId }, now.AddDays(-1), now).FirstOrDefault();
                if (historyData != null)
                    MemoryCache.Set<AdvisorRankingHistory>(cacheKey, historyData, 60);
            }
            return historyData;
        }

        public List<DailyPerformanceResponse> ListDailyPerformance(int advisorId)
        {
            var now = Data.GetDateTimeNow();
            var cacheKey = $"DailyPerformance_{advisorId}_{now.Year}_{now.Month}_{now.Day}";
            var dailyPerformance = MemoryCache.Get<List<DailyPerformanceResponse>>(cacheKey);
            if (dailyPerformance == null)
            {
                var history = Data.ListAdvisorRankingAndProfitHistory(new int[] { advisorId }, now.AddYears(-5), now);
                if (history.Any())
                {
                    dailyPerformance = history.Where(c => c.AdvisorProfitHistory.Any()).Select(c => new DailyPerformanceResponse()
                    {
                        Date = c.ReferenceDate,
                        Equity = c.AdvisorProfitHistory.Where(p => p.OrderStatusType != OrderStatusType.Close).Sum(p => p.TotalDollar)
                    }).ToList();
                    if (dailyPerformance.Count > 1)
                    {
                        for (var i = 1; i < dailyPerformance.Count; ++i)
                            dailyPerformance[i].Variation = dailyPerformance[i].Equity / dailyPerformance[i - 1].Equity - 1;
                    }
                    MemoryCache.Set<List<DailyPerformanceResponse>>(cacheKey, dailyPerformance, 1440);
                }
                else
                    dailyPerformance = new List<DailyPerformanceResponse>();
            }
            return dailyPerformance;
        }

        public List<AdvisorRankingHistory> ListAdvisorsRankingAndProfitForMonthBeginning(IEnumerable<int> advisorsId)
        {
            var now = Data.GetDateTimeNow();
            var cacheKey = $"AdvisorsStartMonthHistory_{now.Year}_{now.Month}";
            var historyData = MemoryCache.Get<List<AdvisorRankingHistory>>(cacheKey);
            if (historyData == null)
            {
                historyData = new List<AdvisorRankingHistory>();
                var referenceDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var monthlyData = Data.ListAdvisorRankingAndProfitHistory(advisorsId, referenceDate, referenceDate.AddMonths(1)).OrderBy(c => c.ReferenceDate);
                foreach (var data in monthlyData)
                {
                    if (!historyData.Any(c => c.UserId == data.UserId))
                        historyData.Add(data);
                }
                if (historyData.Count > 0)
                    MemoryCache.Set<List<AdvisorRankingHistory>>(cacheKey, historyData, 1440);
            }
            return historyData;
        }

        public List<AdvisorRankingHistory> ListAdvisorRankingAndProfitHistory(IEnumerable<int> advisorsId, DateTime startDate, DateTime endDate)
        {
            return Data.ListAdvisorRankingAndProfitHistory(advisorsId, startDate, endDate);
        }
    }
}
