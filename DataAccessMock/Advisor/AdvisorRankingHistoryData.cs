using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorRankingHistoryData : BaseData<AdvisorRankingHistory>, IAdvisorRankingHistoryData<AdvisorRankingHistory>
    {
        public List<AdvisorRankingHistory> ListAdvisorRankingAndProfitHistory(IEnumerable<int> advisorsId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public void SetAdvisorRankingHistory(DateTime referenceDate, IEnumerable<AdvisorRanking> advisorsRanking)
        {
            throw new NotImplementedException();
        }
    }
}
