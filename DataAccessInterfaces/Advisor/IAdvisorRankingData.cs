using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdvisorRankingData<AdvisorRanking> : IBaseData<AdvisorRanking>
    {
        void SetAdvisorRanking(IEnumerable<AdvisorRanking> advisorsRanking);
        List<AdvisorRanking> ListAdvisorsRankingAndProfit(IEnumerable<int> advisorsId, IEnumerable<int> assetsId);
        List<AdvisorRanking> ListAdvisorsRanking(IEnumerable<int> advisorsId);
    }
}
