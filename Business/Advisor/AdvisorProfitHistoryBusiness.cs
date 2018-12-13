using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Advisor
{
    public class AdvisorProfitHistoryBusiness : BaseBusiness<AdvisorProfitHistory, IAdvisorProfitHistoryData<AdvisorProfitHistory>>
    {
        public AdvisorProfitHistoryBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public void SetAdvisorProfitHistory(DateTime referenceDate, IEnumerable<AdvisorProfit> advisorsProfit)
        {
            Data.SetAdvisorProfitHistory(referenceDate, advisorsProfit);
        }
    }
}
