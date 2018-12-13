using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdvisorProfitHistoryData<AdvisorProfitHistory> : IBaseData<AdvisorProfitHistory>
    {
        void SetAdvisorProfitHistory(DateTime referenceDate, IEnumerable<AdvisorProfit> advisorsProfit);
    }
}
