using System;
using System.Collections.Generic;
using System.Text;
using Auctus.DomainObjects.Advisor;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdvisorProfitData<AdvisorProfit> : IBaseData<AdvisorProfit>
    {
        void SetAdvisorProfit(IEnumerable<AdvisorProfit> advisorsProfit);
        List<DomainObjects.Advisor.AdvisorProfit> ListAdvisorProfit(IEnumerable<int> advisorIds, IEnumerable<int> assetIds);
    }
}
