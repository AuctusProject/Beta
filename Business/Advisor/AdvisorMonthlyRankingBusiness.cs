using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
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

namespace Auctus.Business.Advisor
{
    public class AdvisorMonthlyRankingBusiness : BaseBusiness<AdvisorMonthlyRanking, IAdvisorMonthlyRankingData<AdvisorMonthlyRanking>>
    {
        public AdvisorMonthlyRankingBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<AdvisorMonthlyRanking> ListAdvisorsMonthlyRanking(int year, int month)
        {
            return Data.ListAdvisorMonthlyRanking(year, month);
        }

        public List<HallOfFameResponse> ListHallOfFame()
        {
            var cacheKey = "HallOfFameCache";
            var hallOfFame = MemoryCache.Get<List<HallOfFameResponse>>(cacheKey);
            if (hallOfFame == null)
            {
                hallOfFame = new List<HallOfFameResponse>();
                var advisorsMonthlyRanking = Data.ListAdvisorsHallOfFame(3);
                if (advisorsMonthlyRanking.Any())
                {
                    var advisors = AdvisorRankingBusiness.ListAdvisorsFullData();
                    var advisorsFollowers = FollowAdvisorBusiness.ListFollowers(advisorsMonthlyRanking.Select(c => c.UserId).Distinct());
                    var user = GetLoggedUser();
                    var groupedAdvisors = advisorsMonthlyRanking.GroupBy(c => new { c.Year, c.Month });
                    foreach(var data in groupedAdvisors)
                    {
                        var item = new HallOfFameResponse();
                        item.Year = data.Key.Year;
                        item.Month = data.Key.Month;
                        foreach (var advisor in data)
                        {
                            var advisorRanking = advisors.FirstOrDefault(c => c.Id == advisor.UserId);
                            if (advisorRanking != null)
                                item.Advisors.Add(AdvisorRankingBusiness.GetAdvisorResponse(advisorRanking, advisors.Count, advisorsFollowers, user, null, null, null, advisor));
                        }
                        hallOfFame.Add(item);
                    }
                    MemoryCache.Set<List<HallOfFameResponse>>(cacheKey, hallOfFame, 1440);
                }
            }
            return hallOfFame;
        }

        public void SetAdvisorsMonthlyRanking()
        {
            var now = Data.GetDateTimeNow();
            var lastMonth = now.AddMonths(-1);
            var consideredStartDate = new DateTime(lastMonth.Year, lastMonth.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastMonthRanking = ListAdvisorsMonthlyRanking(lastMonth.Year, lastMonth.Month);
            if (lastMonthRanking.Count == 0)
            {
                var advisorsId = AdvisorBusiness.ListAllAdvisors().Select(c => c.Id).Distinct().ToHashSet();
                var monthlyHistory = AdvisorRankingHistoryBusiness.ListAdvisorRankingAndProfitHistory(advisorsId, consideredStartDate, now);
                if (monthlyHistory.Any())
                {
                    var advisorsMonthlyRanking = new List<AdvisorMonthlyRanking>();
                    var consideredEndDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    var advisorsHistory = monthlyHistory.GroupBy(c => c.UserId);
                    foreach(var history in advisorsHistory)
                    {
                        var monthlyAdvisorHistory = history.OrderBy(c => c.ReferenceDate);
                        var firstMonthlyHistory = monthlyAdvisorHistory.FirstOrDefault(c => c.ReferenceDate >= consideredStartDate && c.ReferenceDate < consideredEndDate);
                        var lastMonthlyHistory = monthlyAdvisorHistory.FirstOrDefault(c => c.ReferenceDate >= consideredEndDate);
                        if (firstMonthlyHistory == null || lastMonthlyHistory == null)
                            continue;
                        
                        var firstEquity = firstMonthlyHistory.AdvisorProfitHistory.Where(c => c.OrderStatusType != OrderStatusType.Close).Sum(c => c.TotalDollar);
                        var lastEquity = lastMonthlyHistory.AdvisorProfitHistory.Where(c => c.OrderStatusType != OrderStatusType.Close).Sum(c => c.TotalDollar);
 
                        advisorsMonthlyRanking.Add(new AdvisorMonthlyRanking()
                        {
                            CreationDate = now,
                            Year = lastMonth.Year,
                            Month = lastMonth.Month,
                            UserId = history.Key,
                            AverageReturn = lastEquity / firstEquity - 1
                        });
                    }
                    advisorsMonthlyRanking = advisorsMonthlyRanking.OrderByDescending(c => c.AverageReturn).ThenByDescending(c => c.UserId).ToList();
                    for (var i = 0; i < advisorsMonthlyRanking.Count; ++i)
                        advisorsMonthlyRanking[i].Ranking = i + 1;

                    Data.SetAdvisorMonthlyRanking(advisorsMonthlyRanking);
                }
            }
        }
    }
}
