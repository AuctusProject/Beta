using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdvisorRankingHistoryData<AdvisorRankingHistory> : IBaseData<AdvisorRankingHistory>
    {
        void SetAdvisorRankingHistory(DateTime referenceDate, IEnumerable<AdvisorRanking> advisorsRanking);
        List<AdvisorRankingHistory> ListAdvisorRankingAndProfitHistory(IEnumerable<int> advisorsId, DateTime startDate, DateTime endDate);
    }
}
