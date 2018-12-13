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

namespace Auctus.Business.Advisor
{
    public class AdvisorProfitBusiness : BaseBusiness<AdvisorProfit, IAdvisorProfitData<AdvisorProfit>>
    {
        public AdvisorProfitBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<AdvisorProfit> ListAdvisorProfit(int advisorId)
        {
            var advisors = AdvisorRankingBusiness.ListAdvisorsFullData();
            var advisor = advisors.FirstOrDefault(c => c.Id == advisorId);
            if (advisor == null)
                return Data.ListAdvisorProfit(new int[] { advisorId }, null);
            else
                return advisor.AdvisorProfit;
        }

        public void SetAdvisorProfit(IEnumerable<AdvisorProfit> advisorsProfit)
        {
            Data.SetAdvisorProfit(advisorsProfit);
        }

        public IEnumerable<AdvisorProfit> ListAdvisorProfit(int advisorId, IEnumerable<int> assetIds)
        {
            return ListAdvisorProfit(new List<int>() { advisorId }, assetIds);
        }

        public IEnumerable<AdvisorProfit> ListAdvisorProfit(IEnumerable<int> advisorIds, IEnumerable<int> assetIds)
        {
            return Data.ListAdvisorProfit(advisorIds, assetIds);
        }

        public AdvisorProfit GetBaseUsdAdvisorProfit(int advisorId, DateTime dateTime)
        {
            return new AdvisorProfit()
            {
                AssetId = AssetUSDId,
                OrderCount = 1,
                Status = OrderStatusType.Executed.Value,
                SuccessCount = 0,
                SummedProfitDollar = 0,
                SummedProfitPercentage = 0,
                TotalDollar = VirtualMoney,
                TotalQuantity = VirtualMoney,
                Type = OrderType.Buy.Value,
                UpdateDate = dateTime,
                UserId = advisorId
            };
        }
    }
}
